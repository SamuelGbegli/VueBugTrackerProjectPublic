//DTO for changing an account's password.
export default class PasswordDTO{
  //The user's current password.
  oldPassword: string = "";
  //The user's new password.
  newPassword: string = "";
}
