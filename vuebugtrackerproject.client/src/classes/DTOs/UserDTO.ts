export default class UserDTO{

	// The account's username.
	Username: string = "";
	// The account's email address.
  Emailaddress: string = "";
	// The account's password.
  Password: string = "";
	// The account's ID. Only used when modifying an existing account.
  AccountID: string = "";
	// The JWT of the logged in user. Used to verify the user when making
	// changes.
  AccountJWT: string = "";

}
