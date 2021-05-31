import React, { useEffect, useState } from "react";
import { Button, Form, Input, Row } from "antd";
import {
  saveGeneralInfo,
  getGeneralInfo,
} from "../../services/settings/organizations.services";
import { useAuth } from "../../components/Auth/Auth";

const GeneralInformationPage = () => {
  const { user } = useAuth();
  const organization = user?.organization;
  const [form] = Form.useForm();
  const [editMode, setEditMode] = useState(true);
  const [savedData, setSavedData] = useState({
    name: organization?.name,
    unp: "",
    typeEconomicActivity: "",
    organizationalLegalForm: "",
    govermentForReport: "",
    unitForReport: "",
    address: "",
    position1: "",
    fullName1: "",
    position2: "",
    fullName2: "",
  });
  const [filledFields, setFilledFields] = useState({ ...savedData });

  useEffect(() => {
    const fetchData = async (id: string) => {
      const result = await getGeneralInfo(id);
      if (result.value.unp) {
        setEditMode(false);
        setSavedData(result.value);
        setFilledFields(result.value);
      }
    };
    if (organization) {
      fetchData(organization?.id);
    }
  }, []);

  const onFill = () => {
    form.setFieldsValue({
      name: organization?.name,
      typeEconomicActivity: filledFields.typeEconomicActivity,
      organizationalLegalForm: filledFields.organizationalLegalForm,
      unp: filledFields.unp,
      govermentForReport: filledFields.govermentForReport,
      unitForReport: filledFields.unitForReport,
      address: filledFields.address,
      position1: filledFields.position1,
      fullName1: filledFields.fullName1,
      position2: filledFields.position2,
      fullName2: filledFields.fullName2,
    });
  };

  const handlerSaveResults = async () => {
    if (
      Object.values(filledFields).find((el) => el?.length === 0) === undefined
    ) {
      setSavedData(filledFields);
      setEditMode(false);
      const data = {
        id: organization?.id,
        UNP: filledFields.unp,
        TypeEconomicActivity: filledFields.typeEconomicActivity,
        OrganizationalLegalForm: filledFields.organizationalLegalForm,
        GovermentForReport: filledFields.govermentForReport,
        UnitForReport: filledFields.unitForReport,
        Address: filledFields.address,
        Position1: filledFields.position1,
        FullName1: filledFields.fullName1,
        Position2: filledFields.position2,
        FullName2: filledFields.fullName2,
      };
      await saveGeneralInfo(data);
    }
  };

  const inputFieldHandler = (e: string, currentField: string) => {
    setFilledFields({ ...filledFields, [currentField]: e });
  };

  const handlerCancelButton = () => {
    setFilledFields(savedData);
    onFill();
  };

  useEffect(() => {
    onFill();
  }, [filledFields]);

  return (
    <Form
      style={{ maxWidth: 900, margin: "auto" }}
      form={form}
      layout="vertical"
    >
      <Row justify="end">
        <Button
          type="primary"
          htmlType="submit"
          style={{ marginBottom: 10 }}
          onClick={() => setEditMode(true)}
          disabled={editMode}
        >
          Изменить данные
        </Button>
      </Row>

      <Form.Item
        name="name"
        label="Наша организация"
        rules={[{ required: true }]}
      >
        <Input
          disabled={true}
          // onChange={(e) => inputFieldHandler(e.target.value, "name")}
        />
      </Form.Item>
      <Form.Item
        name="unp"
        label="УНП организации"
        rules={[{ required: true }]}
      >
        <Input
          disabled={!editMode}
          onChange={(e) => inputFieldHandler(e.target.value, "unp")}
          value={filledFields.unp}
        />
      </Form.Item>
      <Form.Item
        name="typeEconomicActivity"
        label="Вид экономической деятельности организации"
        rules={[{ required: true }]}
      >
        <Input
          disabled={!editMode}
          onChange={(e) =>
            inputFieldHandler(e.target.value, "typeEconomicActivity")
          }
          value={filledFields.typeEconomicActivity}
        />
      </Form.Item>
      <Form.Item
        name="organizationalLegalForm"
        label="Организационно-правовая форма"
        rules={[{ required: true }]}
      >
        <Input
          disabled={!editMode}
          onChange={(e) =>
            inputFieldHandler(e.target.value, "organizationalLegalForm")
          }
          value={filledFields.organizationalLegalForm}
        />
      </Form.Item>
      <Form.Item
        name="govermentForReport"
        label="Орган управления для заполнения форм в создании отчета"
        rules={[{ required: true }]}
      >
        <Input
          disabled={!editMode}
          onChange={(e) =>
            inputFieldHandler(e.target.value, "govermentForReport")
          }
          value={filledFields.govermentForReport}
        />
      </Form.Item>
      <Form.Item
        name="unitForReport"
        label="Единица измерения для заполнения форм в создании отчета"
        rules={[{ required: true }]}
      >
        <Input
          disabled={!editMode}
          onChange={(e) => inputFieldHandler(e.target.value, "unitForReport")}
          value={filledFields.unitForReport}
        />
      </Form.Item>
      <Form.Item
        name="address"
        label="Адрес организации"
        rules={[{ required: true }]}
      >
        <Input
          disabled={!editMode}
          onChange={(e) => inputFieldHandler(e.target.value, "address")}
          value={filledFields.address}
        />
      </Form.Item>
      <Form.Item
        name="position1"
        label="Должность 1"
        rules={[{ required: true }]}
      >
        <Input
          disabled={!editMode}
          onChange={(e) => inputFieldHandler(e.target.value, "position1")}
          value={filledFields.position1}
          placeholder="Директор"
        />
      </Form.Item>
      <Form.Item
        name="fullName1"
        label="ФИО Должности 1"
        rules={[{ required: true }]}
      >
        <Input
          disabled={!editMode}
          onChange={(e) => inputFieldHandler(e.target.value, "fullName1")}
          value={filledFields.fullName1}
        />
      </Form.Item>
      <Form.Item
        name="position2"
        label="Должность 2"
        rules={[{ required: true }]}
      >
        <Input
          disabled={!editMode}
          onChange={(e) => inputFieldHandler(e.target.value, "position2")}
          value={filledFields.position2}
          placeholder="Главный бухгалтер"
        />
      </Form.Item>
      <Form.Item
        name="fullName2"
        label="ФИО Должности 2"
        rules={[{ required: true }]}
      >
        <Input
          disabled={!editMode}
          onChange={(e) => inputFieldHandler(e.target.value, "fullName2")}
          value={filledFields.fullName2}
        />
      </Form.Item>

      <Row justify="end">
        <Button
          type="primary"
          htmlType="submit"
          style={{ marginRight: 40 }}
          onClick={() => handlerSaveResults()}
          disabled={!editMode}
        >
          Сохранить
        </Button>

        <Button
          type="primary"
          htmlType="submit"
          disabled={!editMode}
          onClick={() => handlerCancelButton()}
        >
          Отмена
        </Button>
      </Row>
    </Form>
  );
};
export default GeneralInformationPage;
