import React, { useEffect, useState, useRef } from "react";
import {
  Table,
  Input,
  Form,
  Select,
  Switch,
  Button,
  Divider,
  Popconfirm,
  Space,
} from "antd";
import { SearchOutlined } from "@ant-design/icons";
import Highlighter from "react-highlight-words";

import "./OrganizationTable.scss";

import {
  Organization,
  TypeActivity,
  Filter,
} from "../../../../models/common-types";
import {
  addOrganization,
  getOrganizations,
  getTypesActivities,
  editOrganization,
  deleteOrganization,
} from "../../../../services/settings/organizations.services";
import {
  yesNo,
  isIndustrial,
  regions,
  industrial,
} from "../../../../constants/constants";
import { DeleteFilled, EditFilled } from "@ant-design/icons";
import { openNotification } from "../../../../helpers/helpers";

const { Option, OptGroup } = Select;

type GroupActivities = {
  industrial: Array<TypeActivity>;
  notIndustrial: Array<TypeActivity>;
};

const OrganizationTable = () => {
  const [tableForm] = Form.useForm();
  const [createForm] = Form.useForm();
  const [data, setData] = useState<Array<Organization>>([]);
  const [types, setTypes] = useState<GroupActivities>({
    industrial: [],
    notIndustrial: [],
  });
  const [categoryFilterArr, setCategoryFilterArr] = useState<Array<any>>([]);
  const [editingKey, setEditingKey] = useState("");
  const [loading, setLoading] = useState<boolean>(false);
  const [searchText, setSearchText] = useState<any>("");
  const [searchedColumn, setSearchedColumn] = useState<string>("");
  const [page, setPage] = useState<number>();
  const [pageSize, setPageSize] = useState<number | undefined>();

  useEffect(() => {
    setLoading(true);
    (async function () {
      const newData = await getOrganizations();
      const newTypes = await getTypesActivities();
      const typesForSelect: GroupActivities = {
        industrial: [],
        notIndustrial: [],
      };
      newTypes.forEach((item: TypeActivity) =>
        item.isIndustrial
          ? typesForSelect.industrial.push(item)
          : typesForSelect.notIndustrial.push(item),
      );
      const categoryFilter: Array<any> = newTypes.map((item: any) => {
        return { text: item.name, value: item.name };
      });
      setData(newData);
      setTypes(typesForSelect);
      setCategoryFilterArr(categoryFilter);
      setLoading(false);
    })();
  }, []);

  const edit = (record: Organization) => {
    createForm.setFieldsValue({ ...record });
    setEditingKey(record.id);

    window.scrollTo({
      top: 0,
      behavior: "smooth",
    });
  };

  const deleteOrg = async (id: string) => {
    setLoading(true);
    const res = await deleteOrganization(id);
    if (res.isSuccess) {
      const organizations = await getOrganizations();
      setData(organizations);
    } else {
      const type = "error";
      const title = "Ошибка удаления организации";
      const msg =
        "Возможно организация уже где-то используется. Также, пожалуйста, проверьте интернет-подключение.";
      openNotification(title, msg, type);
    }
    setLoading(false);
  };

  const reset = () => {
    setEditingKey("");
    createForm.setFieldsValue({
      name: "",
      isState: false,
      isHolding: false,
      region: null,
      typeActivityId: null,
    });
  };

  const validateMessages = {
    required: "Поле ${label} не должно пустым!",
  };

  const onFinish = async (e: any) => {
    if (editingKey) {
      setLoading(true);
      if (e.isState === undefined) {
        e.isState = false;
      }
      if (e.isHolding === undefined) {
        e.isHolding = false;
      }
      e.id = editingKey;
      const res = await editOrganization(e);
      if (res.isSuccess) {
        const organizations = await getOrganizations();
        setData(organizations);
      } else {
        const type = "error";
        const title = "Ошибка редактирования организации";
        const msg = "Пожалуйста, проверьте интернет-подключение.";
        openNotification(title, msg, type);
      }
      reset();
      setLoading(false);
    } else {
      setLoading(true);
      if (e.isState === undefined) {
        e.isState = false;
      }
      if (e.isHolding === undefined) {
        e.isHolding = false;
      }
      const res = await addOrganization(e);
      if (res.isSuccess) {
        const organizations = await getOrganizations();
        setData(organizations);
      } else {
        const type = "error";
        const title = "Ошибка создания организации";
        const msg = "Пожалуйста, проверьте интернет-подключение.";
        openNotification(title, msg, type);
      }
      reset();
      setLoading(false);
    }
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
      return record[dataIndex]
        ? record[dataIndex]
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
    render: (text: string) => {
      return searchedColumn === dataIndex ? (
        <Highlighter
          highlightStyle={{ backgroundColor: "#ffc069", padding: 0 }}
          searchWords={[searchText]}
          autoEscape
          textToHighlight={text.toString()}
        />
      ) : (
        text
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
      title: "№",
      key: "index",
      render: (text: any, record: any, index: any) => (
        <div style={{ textAlign: "center" }}>
          {((page ?? 1) - 1) * (pageSize ?? 10) + index + 1}
        </div>
      ),
      width: 70,
    },
    {
      title: "Название",
      dataIndex: "name",
      ...getColumnSearchProps("name"),
      sorter: (a: Organization, b: Organization) => {
        if (a.name < b.name) {
          return -1;
        }
        if (a.name > b.name) {
          return 1;
        }
        return 0;
      },
      defaultSortOrder: "ascend",
    },
    {
      title: "Тип",
      dataIndex: "type",
      filters: industrial,
      render: (_: any, record: Organization) => {
        const name: string =
          isIndustrial[Number(record.typeActivity.isIndustrial)];
        return <span>{name}</span>;
      },
      onFilter: (value: boolean, record: Organization) => {
        return record.typeActivity.isIndustrial === value;
      },
    },
    {
      title: "Категория",
      dataIndex: "category",
      filters: categoryFilterArr,
      render: (_: any, record: Organization) => {
        const name: string = record.typeActivity.name;
        return <span>{name}</span>;
      },
      onFilter: (value: string, record: Organization) =>
        record.typeActivity.name.indexOf(value) === 0,
    },
    {
      title: "Область",
      dataIndex: "region",
      filters: regions,
      render: (_: any, record: Organization) => {
        const name: string = regions[Number(record.region)].text;
        return <span>{name}</span>;
      },
      onFilter: (value: string, record: Organization) => {
        const filter: any = regions.find(
          (item: Filter) => item.value === value,
        );
        return record.region === regions.indexOf(filter);
      },
    },
    {
      title: "Процент гос. собственности 50% и >",
      width: 130,
      dataIndex: "isState",
      filters: yesNo,
      render: (_: any, record: Organization) => {
        const name: string = yesNo[Number(record.isState)].text;
        return <span>{name}</span>;
      },
      onFilter: (value: string, record: Organization) => {
        const filter: any = yesNo.find((item: Filter) => item.value === value);
        return Number(record.isState) === yesNo.indexOf(filter);
      },
    },
    {
      title: "Холдинг ДО организаций",
      width: 130,
      dataIndex: "isHolding",
      filters: yesNo,
      render: (_: any, record: Organization) => {
        const name: string = yesNo[Number(record.isHolding)].text;
        return <span>{name}</span>;
      },
      onFilter: (value: string, record: Organization) => {
        const filter: any = yesNo.find((item: Filter) => item.value === value);
        return Number(record.isHolding) === yesNo.indexOf(filter);
      },
    },
    {
      title: "Действия",
      dataIndex: "operation",
      render: (_: any, record: Organization) => {
        return (
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
            <Popconfirm
              disabled={editingKey !== ""}
              title={`Вы уверены, что хотите удалить организацию ${record.name} ?`}
              placement={"left"}
              className={"action-button-link"}
              onConfirm={() => deleteOrg(record.id)}
              okText={"Да"}
              cancelText={"Нет"}
            >
              <button
                disabled={editingKey !== ""}
                className={"action-button-link"}
              >
                <DeleteFilled title={"Удалить"} style={{ fontSize: "16px" }} />
              </button>
            </Popconfirm>
          </>
        );
      },
    },
  ];

  // noinspection RequiredAttributes
  return (
    <>
      <Divider
        style={{ paddingTop: "15px", marginTop: "0" }}
        orientation={"left"}
      >
        {editingKey ? "Редактирование организации" : "Создание организации"}
      </Divider>
      <Form
        layout={"inline"}
        form={createForm}
        className={"inline-table-form"}
        onFinish={onFinish}
        validateMessages={validateMessages}
      >
        <Form.Item name="name" label="Название" rules={[{ required: true }]}>
          <Input
            style={{ minWidth: "200px" }}
            placeholder="Введите название..."
          />
        </Form.Item>
        <Form.Item
          name="typeActivityId"
          label="Категория"
          rules={[{ required: true }]}
        >
          <Select
            style={{ minWidth: "200px" }}
            placeholder="Выберите категорию..."
          >
            <OptGroup key={"prom-group"} label={"Промышленные"}>
              {types.industrial.map((item) => (
                <Option key={item.id} value={item.id}>
                  {item.name}
                </Option>
              ))}
            </OptGroup>
            <OptGroup key={"not-prom-group"} label={"Непромышленные"}>
              {types.notIndustrial.map((item) => (
                <Option key={item.id} value={item.id}>
                  {item.name}
                </Option>
              ))}
            </OptGroup>
          </Select>
        </Form.Item>
        <Form.Item name="region" label="Область" rules={[{ required: true }]}>
          <Select
            style={{ minWidth: "200px" }}
            placeholder="Выберите область..."
          >
            {regions.map((item, i) => (
              <Option key={i} value={i}>
                {item.text}
              </Option>
            ))}
          </Select>
        </Form.Item>
        <Form.Item
          valuePropName="checked"
          name="isState"
          label="Процент гос. собственности 50% и >"
        >
          <Switch
            defaultChecked={false}
            style={{ width: "55px" }}
            checkedChildren={"Да"}
            unCheckedChildren={"Нет"}
          />
        </Form.Item>
        <Form.Item
          valuePropName="checked"
          name="isHolding"
          label="Холдинг ДО организаций"
        >
          <Switch
            defaultChecked={false}
            style={{ width: "55px" }}
            checkedChildren={"Да"}
            unCheckedChildren={"Нет"}
          />
        </Form.Item>
        <Form.Item>
          {editingKey ? (
            <>
              <Button
                style={{
                  width: "110px",
                  textAlign: "center",
                  marginRight: "20px",
                }}
                type="primary"
                htmlType="submit"
              >
                Сохранить
              </Button>
              <Button
                style={{
                  width: "110px",
                  textAlign: "center",
                }}
                type="default"
                htmlType="button"
                onClick={reset}
              >
                Отменить
              </Button>
            </>
          ) : (
            <Button
              style={{ width: "110px", textAlign: "center" }}
              type="primary"
              htmlType="submit"
            >
              Создать
            </Button>
          )}
        </Form.Item>
      </Form>
      <Divider />
      <div style={{ padding: "0 15px" }}>
        <Form form={tableForm} component={false}>
          <Table
            rowKey="id"
            bordered
            dataSource={data}
            columns={columns}
            loading={loading}
            rowClassName={(record) =>
              editingKey === record.id
                ? "editable-row row-edit"
                : "editable-row"
            }
            pagination={{
              pageSize: 100,
              disabled: editingKey !== "",
              onChange: (pageNumber, pageSize) => {
                reset();
                setPage(pageNumber);
                setPageSize(pageSize);
              },
              showTotal: (total) => {
                return <span>Всего {total}</span>;
              },
            }}
            scroll={{ y: "none" }}
          />
        </Form>
      </div>
    </>
  );
};

export default OrganizationTable;
