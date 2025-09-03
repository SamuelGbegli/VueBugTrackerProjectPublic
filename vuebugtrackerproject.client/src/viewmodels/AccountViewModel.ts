import AccountRole from "@/enumConsts/Role";

export default class AccountViewModel{

	// Unique identifier for the account.
	id: string = "";
	// The account's username.
	username: string = "";
	// The icon that will be shown with the account's username.
	icon: string = "";
	// The role and privileges the account has in the application.
	role: number = AccountRole.Normal;
	// If true, the user cannot login with the account.
	suspended: boolean = false;
	// The date and time the account was created.
	dateCreated: Date = new Date();
}
