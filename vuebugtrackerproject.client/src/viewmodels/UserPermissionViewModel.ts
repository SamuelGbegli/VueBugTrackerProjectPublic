export default class UserPermissionViewModel{

	// Unique identifier for the class.
	id: string;
	// The ID of the user who is granted access to the project.
	accountID: string;
	// The name of the user who is granted access to the project.
	accountName: string;
	// The icon of the user who is granted access to the project.
	accountIcon: string;
	// What the user can do with the project.
	permission: ProjectPermission;
}
