export default class AccountContainer{

	// The number of accounts in the project.
	totalAccounts: number;
	// The number of pages of accounts the user can see.
	pages: number;
	// The current page of accounts that are visible.
	currentPage: number;
	// The current list of accounts.
	accounts: Account[];
}
