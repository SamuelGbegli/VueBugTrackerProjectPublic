export default class UserPermissionContainer{

	// The number of user permissions in the project.
	totalPermissions: number;
	// The number of pages of user permissions the user can see.
	pages: number;
	// The current page of user permissions that are visible.
	currentPage: number;
	// The current list of user permissions.
	userPermissions: UserPermission[];
}
