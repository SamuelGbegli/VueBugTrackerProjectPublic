//Const/enum for determining what buttons appear on an error banner

const ErrorPromptType = {
  //Only shows a login button
  LoginButtonOnly: 0,
  //Shows buttons to log in and go to the previous page
  LoginAndGoBackButtons: 1,
  //Only shows button to go back
  GoBackButtonOnly:2,
  //Shows button to reload the page
  ReloadButton: 3

} as const;

export default ErrorPromptType;
