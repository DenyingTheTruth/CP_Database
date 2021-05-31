import React, { useEffect, useState, useRef } from "react";
import {
  Button,
  Divider,
  Form,
  Popconfirm,
  Select,
  Table,
  Input,
  Space,
  Modal,
} from "antd";

import {
  DeleteFilled,
  EditFilled,
  ReloadOutlined,
  RollbackOutlined,
  SaveOutlined,
  SearchOutlined,
} from "@ant-design/icons";
import Highlighter from "react-highlight-words";

import {
  Organization,
  OrganizationUser,
} from "../../../../models/common-types";
import {
  addOrganizationUser,
  deleteOrganizationUser,
  editOrganizationUser,
  getOrganizationsUsers,
  getOrganizationsWithoutUsers,
  reCreatePassOrgUser,
} from "../../../../services/settings/users.services";
import { openNotification } from "../../../../helpers/helpers";

const { Option } = Select;

interface EditableCellProps extends React.HTMLAttributes<HTMLElement> {
  editing: boolean;
  dataIndex: string;
  title: any;
  save: (id: React.Key) => {};
  record: OrganizationUser;
  index: number;
  children: React.ReactNode;
}

const EditableCell: React.FC<EditableCellProps> = ({
  editing,
  dataIndex,
  title,
  save,
  record,
  index,
  children,
  ...restProps
}) => {
  return (
    <td {...restProps}>
      {editing ? (
        <Form.Item
          name={dataIndex}
          style={{ margin: 0 }}
          rules={[
            {
              required: true,
              message: `Please Input ${title}!`,
            },
          ]}
        >
          <Input autoFocus={true} onPressEnter={() => save(record.id)} />
        </Form.Item>
      ) : (
        children
      )}
    </td>
  );
};

const UsersTable = () => {
  const [tableForm] = Form.useForm();
  const [createForm] = Form.useForm();
  const [organizations, setOrganizations] = useState<Array<Organization>>([]);
  const [organizationsUsers, setOrganizationsUsers] = useState<
    Array<OrganizationUser>
  >([]);
  const [loading, setLoading] = useState<boolean>(false);
  const [searchText, setSearchText] = useState<any>("");
  const [searchedColumn, setSearchedColumn] = useState<string>("");
  const [isModalVisible, setIsModalVisible] = useState(false);
  const [userName, setUserName] = useState<string>("");
  const [password, setPassword] = useState<string>("");
  const [editingKey, setEditingKey] = useState("");
  const isEditing = (record: OrganizationUser) => record.id === editingKey;

  const edit = (record: Partial<OrganizationUser> & { id: React.Key }) => {
    tableForm.setFieldsValue({ ...record });
    setEditingKey(record.id);
  };

  const cancel = () => {
    setEditingKey("");
  };

  const save = async (id: React.Key) => {
    try {
      setLoading(true);
      const row = (await tableForm.validateFields()) as OrganizationUser;

      const newData = [...organizationsUsers];
      const index = newData.findIndex((item) => id === item.id);
      const item = newData[index];
      const newItem = {
        ...item,
        ...row,
      };
      newData.splice(index, 1, newItem);
      const res = await editOrganizationUser(newItem);
      if (res.isSuccess) {
        setOrganizationsUsers(newData);
        setEditingKey("");
        const type = "success";
        const title = "Логин пользователя успешно изменён";
        const msg = "";
        openNotification(title, msg, type);
      } else {
        if (res.value === 1488) {
          const type = "error";
          const title = "Ошибка изменения логина пользователя";
          const msg = "Данный логин пользователя уже существует.";
          openNotification(title, msg, type);
        } else {
          const type = "error";
          const title = "Ошибка изменения логина пользователя";
          const msg = "Пожалуйста, проверьте интернет-подключение.";
          openNotification(title, msg, type);
        }
      }
      setLoading(false);
    } catch (errInfo) {
      console.log("Validate Failed:", errInfo);
    }
  };

  useEffect(() => {
    setLoading(true);
    (async function () {
      const newOrganizations = await getOrganizationsWithoutUsers();
      const organizationsUsers = await getOrganizationsUsers();
      setOrganizations(newOrganizations);
      setOrganizationsUsers(organizationsUsers);
      setLoading(false);
    })();
  }, []);

  const reset = () => {
    createForm.setFieldsValue({
      organizationId: null,
    });
  };

  const deleteUser = async (id: string) => {
    setLoading(true);
    const res = await deleteOrganizationUser(id);
    if (res.isSuccess) {
      const organizationsUsers = await getOrganizationsUsers();
      const newOrganizations = await getOrganizationsWithoutUsers();
      setOrganizations(newOrganizations);
      setOrganizationsUsers(organizationsUsers);
    } else {
      const type = "error";
      const title = "Ошибка удаления пользователя";
      const msg =
        "Возможно этот пользователь уже где-то используется. Также, пожалуйста, проверьте интернет-подключение.";
      openNotification(title, msg, type);
    }
    setLoading(false);
  };

  const reCreatePass = async (id: string) => {
    setLoading(true);
    const res = await reCreatePassOrgUser(id);
    if (res.isSuccess) {
      const organizationsUsers = await getOrganizationsUsers();
      setOrganizationsUsers(organizationsUsers);
    } else {
      const type = "error";
      const title = "Ошибка удаления пользователя";
      const msg = "Пожалуйста, проверьте интернет-подключение.";
      openNotification(title, msg, type);
    }
    setLoading(false);
  };

  const validateMessages = {
    required: "Поле ${label} не должно пустым!",
  };

  const onFinish = async (e: any) => {
    setLoading(true);
    const res = await addOrganizationUser(e);
    if (res.isSuccess) {
      const organizationsUsers = await getOrganizationsUsers();
      const newOrganizations = await getOrganizationsWithoutUsers();
      setOrganizations(newOrganizations);
      setOrganizationsUsers(organizationsUsers);
      reset();
      setUserName(res.value.login);
      setPassword(res.value.password);
      setIsModalVisible(true);
    } else {
      const type = "error";
      const title = "Ошибка создания пользователя";
      const msg = "Пожалуйста, проверьте интернет-подключение.";
      openNotification(title, msg, type);
    }
    setLoading(false);
  };

  const inputRef = useRef<any>();

  const getColumnSearchProps = (dataIndex: any) => ({
    filterDropdown: ({
      setSelectedKeys,
      selectedKeys,
      confirm,
      clearFilters,
    }: {
      setSelectedKeys: (selectedKeys: any[]) => void;
      selectedKeys: any[];
      confirm: () => void;
      clearFilters: () => void;
    }) => (
      <div style={{ padding: 8 }}>
        <Input
          ref={inputRef}
          placeholder={`Поиск`}
          value={selectedKeys[0]}
          onChange={(e) =>
            setSelectedKeys(e.target.value ? [e.target.value] : [])
          }
          onPressEnter={() => handleSearch(selectedKeys, confirm, dataIndex)}
          style={{ width: 188, marginBottom: 8, display: "block" }}
        />
        <Space>
          <Button
            type="primary"
            onClick={() => handleSearch(selectedKeys, confirm, dataIndex)}
            icon={<SearchOutlined />}
            size="small"
            style={{ width: 90 }}
          >
            Поиск
          </Button>
          <Button
            onClick={() => handleReset(clearFilters)}
            size="small"
            style={{ width: 90 }}
          >
            Сброс
          </Button>
        </Space>
      </div>
    ),
    filterIcon: (filtered: any) => (
      <SearchOutlined style={{ color: filtered ? "#1890ff" : undefined }} />
    ),
    onFilter: (value: any, record: any) => {
      return record[dataIndex]?.name
        ? record[dataIndex].name
            .toString()
            .toLowerCase()
            .includes(value.toLowerCase())
        : "";
    },
    onFilterDropdownVisibleChange: (visible: any) => {
      if (visible) {
        setTimeout(() => inputRef.current.focus(), 100);
      }
    },
    render: (text: any) => {
      return searchedColumn === dataIndex ? (
        <Highlighter
          highlightStyle={{ backgroundColor: "#ffc069", padding: 0 }}
          searchWords={[searchText]}
          autoEscape
          textToHighlight={text?.name.toString()}
        />
      ) : (
        text?.name
      );
    },
  });

  const handleSearch = (
    selectedKeys: any[],
    confirm: any,
    dataIndex: string,
  ) => {
    confirm();
    setSearchText(selectedKeys[0]);
    setSearchedColumn(dataIndex);
  };

  const handleReset = (clearFilters: () => void) => {
    clearFilters();
    setSearchText("");
  };

  const columns: any = [
    {
      title: "Подотчетная организация",
      dataIndex: "organization",
      editable: false,
      sorter: (a: OrganizationUser, b: OrganizationUser) => {
        if (
          a.organization &&
          b.organization &&
          a.organization?.name < b.organization?.name
        ) {
          return -1;
        }
        if (
          a.organization &&
          b.organization &&
          a.organization?.name > b.organization?.name
        ) {
          return 1;
        }
        return 0;
      },
      defaultSortOrder: "ascend",
      ...getColumnSearchProps("organization"),
    },
    {
      title: "Логин",
      dataIndex: "userName",
      editable: true,
    },
    {
      title: "Пароль",
      dataIndex: "password",
      editable: false,
    },
    {
      title: "Действия",
      dataIndex: "operation",
      editable: false,
      render: (_: any, record: OrganizationUser) => {
        const editing = isEditing(record);
        return editing ? (
          <span>
            <button
              className={"action-button-link"}
              onClick={() => save(record.id)}
              style={{ marginRight: 8 }}
            >
              <SaveOutlined title={"Сохранить"} style={{ fontSize: "16px" }} />
            </button>
            <Popconfirm
              title="Вы уверены, что хотите отменить?"
              placement={"left"}
              className={"action-button-link"}
              onConfirm={cancel}
              okText={"Да"}
              cancelText={"Нет"}
            >
              <a>
                <RollbackOutlined
                  title={"Отмена"}
                  style={{ fontSize: "16px" }}
                />
              </a>
            </Popconfirm>
          </span>
        ) : (
          <>
            <button
              className={"action-button-link"}
              disabled={editingKey !== ""}
              onClick={() => edit(record)}
            >
              <EditFilled
                title={"Редактировать"}
                style={{ fontSize: "16px" }}
              />
            </button>
            <button
              className={"action-button-link"}
              onClick={() => reCreatePass(record.id)}
            >
              <ReloadOutlined
                title={"Пересоздать пароль"}
                style={{ fontSize: "16px" }}
              />
            </button>
            <Popconfirm
              title={`Вы уверены, что хотите удалить организацию ${record.userName} ?`}
              placement={"left"}
              className={"action-button-link"}
              onConfirm={() => deleteUser(record.id)}
              okText={"Да"}
              cancelText={"Нет"}
            >
              <button className={"action-button-link"}>
                <DeleteFilled title={"Удалить"} style={{ fontSize: "16px" }} />
              </button>
            </Popconfirm>
          </>
        );
      },
    },
  ];

  const handleClose = () => {
    setIsModalVisible(false);
    setUserName("");
    setPassword("");
  };

  const mergedColumns = columns.map((col: any) => {
    if (!col.editable) {
      return col;
    }
    return {
      ...col,
      onCell: (record: OrganizationUser) => ({
        record,
        save,
        dataIndex: col.dataIndex,
        title: col.title,
        editing: isEditing(record),
      }),
    };
  });

  // const copyUserName = () => {
  //   navigator.clipboard.writeText(userName);
  //   const type = "success";
  //   const title = "Имя пользователя скопировано в буфер обмена";
  //   const msg = "";
  //   openNotification(title, msg, type);
  // };
  //
  // const copyPass = () => {
  //   navigator.clipboard.writeText(password);
  //   const type = "success";
  //   const title = "Пароль скопирован в буфер обмена";
  //   const msg = "";
  //   openNotification(title, msg, type);
  // };

  // noinspection RequiredAttributes
  return (
    <>
      <Divider
        style={{ paddingTop: "15px", marginTop: "0" }}
        orientation={"left"}
      >
        Создание пользователя
      </Divider>
      <Form
        layout={"inline"}
        form={createForm}
        className={"inline-table-form"}
        onFinish={onFinish}
        validateMessages={validateMessages}
      >
        <Form.Item
          name="organizationId"
          label="Подотчётная организация"
          rules={[{ required: true }]}
        >
          <Select
            showSearch
            optionFilterProp="children"
            filterOption={(input, option: any) =>
              option.children.toLowerCase().indexOf(input.toLowerCase()) >= 0
            }
            style={{ minWidth: "340px" }}
            placeholder="Выберите организацию..."
          >
            {organizations.map((item) => (
              <Option key={item.id} value={item.id}>
                {item.name}
              </Option>
            ))}
          </Select>
        </Form.Item>
        <Form.Item>
          <Button
            style={{ width: "110px", textAlign: "center" }}
            type="primary"
            htmlType="submit"
            disabled={loading}
          >
            Создать
          </Button>
        </Form.Item>
      </Form>
      <Divider />
      <div style={{ padding: "0 15px" }}>
        <Form form={tableForm} component={false}>
          <Table
            rowKey="id"
            bordered
            dataSource={organizationsUsers}
            columns={mergedColumns}
            loading={loading}
            components={{
              body: {
                cell: EditableCell,
              },
            }}
            rowClassName="editable-row"
            pagination={{ pageSize: 100 }}
            scroll={{ y: "none" }}
          />
        </Form>
      </div>
      <Modal
        title="Создан новый пользователь"
        visible={isModalVisible}
        onCancel={handleClose}
        footer={[
          <Button type="primary" onClick={handleClose}>
            Закрыть
          </Button>,
        ]}
      >
        <div
          style={{
            display: "flex",
            justifyContent: "space-between",
            marginBottom: "10px",
          }}
        >
          <span>Имя пользователя:</span>
          <div style={{ display: "flex", justifyContent: "flex-end" }}>
            <span>{userName}</span>
            {/*<Button style={{ marginLeft: "10px" }} onClick={copyUserName}>*/}
            {/*  Копировать*/}
            {/*</Button>*/}
          </div>
        </div>
        <div style={{ display: "flex", justifyContent: "space-between" }}>
          <span>Пароль:</span>
          <div style={{ display: "flex", justifyContent: "flex-end" }}>
            <span>{password}</span>
            {/*<Button style={{ marginLeft: "10px" }} onClick={copyPass}>*/}
            {/*  Копировать*/}
            {/*</Button>*/}
          </div>
        </div>
      </Modal>
    </>
  );
};

export default UsersTable;
