import { url } from "../../constants/constants";
import { token } from "../../helpers/helpers";
import { GeneralInformation, Organization } from "../../models/common-types";

async function getOrganizations() {
  return await fetch(`${url}/organizations`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((res) => {
      return res;
    })
    .catch((err) => {
      console.log(err);
    });
}

async function getTypesActivities() {
  return await fetch(`${url}/type-activities`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((res) => {
      if (!res.errorText) {
        return res;
      }
    })
    .catch((err) => {
      console.log(err);
    });
}

async function addOrganization(organization: Organization) {
  return await fetch(`${url}/organizations`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(organization),
  })
    .then((res) => res.json())
    .then((result) => result)
    .catch((err) => err);
}

async function editOrganization(organization: Organization) {
  return await fetch(`${url}/organizations`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(organization),
  })
    .then((res) => res.json())
    .then((result) => result)
    .catch((err) => err);
}

async function deleteOrganization(id: string) {
  return await fetch(`${url}/organizations/${id}`, {
    method: "DELETE",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((result) => result)
    .catch((err) => err);
}
async function saveGeneralInfo(generalInformation: GeneralInformation) {
  return await fetch(`${url}/organizations/save-general-info`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(generalInformation),
  })
    .then((res) => res.json())
    .then((result) => result)
    .catch((err) => err);
}

async function getGeneralInfo(id: string) {
  return await fetch(`${url}/organizations/get-general-info/${id}`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((res) => {
      if (!res.errorText) {
        return res;
      }
    })
    .catch((err) => {
      console.log(err);
    });
}

export {
  getOrganizations,
  getTypesActivities,
  addOrganization,
  editOrganization,
  deleteOrganization,
  saveGeneralInfo,
  getGeneralInfo,
};
