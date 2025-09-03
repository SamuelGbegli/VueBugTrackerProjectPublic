export default class PasswordResetToken{
  // The ID of the user.
  accountID: string = "";

  // The token used to authorise the password reset.
  password: string = "";

  // The user's new password.
  passwordResetToken: string = "";
}
