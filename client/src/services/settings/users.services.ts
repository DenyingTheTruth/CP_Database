import { OrganizationUser } from "../../models/common-types";
import { url } from "../../constants/constants";
import { token } from "../../helpers/helpers";

async function getOrganizationsUsers() {
  return await fetch(`${url}/organizations/users`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((result) => result)
    .catch((err) => err);
}

async function getOrganizationsWithoutUsers() {
  return await fetch(`${url}/organizations/free`, {
    method: "GET",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((result) => result)
    .catch((err) => err);
}

async function addOrganizationUser(user: OrganizationUser) {
  return await fetch(`${url}/organizations/users`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
    body: JSON.stringify(user),
  })
    .then((res) => res.json())
    .then((result) => result)
    .catch((err) => err);
}

async function editOrganizationUser(user: OrganizationUser) {
  return await fetch(`${url}/organizations/users/${user.id},${user.userName}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((result) => result)
    .catch((err) => err);
}

async function deleteOrganizationUser(id: string) {
  return await fetch(`${url}/organizations/users/${id}`, {
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

async function reCreatePassOrgUser(id: string) {
  return await fetch(`${url}/organizations/users/reset-password/${id}`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token()}`,
    },
  })
    .then((res) => res.json())
    .then((result) => result)
    .catch((err) => err);
}

export {
  getOrganizationsUsers,
  getOrganizationsWithoutUsers,
  addOrganizationUser,
  editOrganizationUser,
  deleteOrganizationUser,
  reCreatePassOrgUser,
};
