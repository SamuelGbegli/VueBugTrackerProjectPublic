import AccountRole from "@/enumConsts/Role";

//DTO to update an account's role.
export default class RoleDTO{
  //The ID of the account whose role will be updated.
  accountID: string = "";
  //The role the user will have.
  role: number = AccountRole.Normal;
}
