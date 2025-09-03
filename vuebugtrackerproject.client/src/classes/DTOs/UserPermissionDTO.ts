import ProjectPermission from "@/enumConsts/ProjectPermission";

export default class UserPermissionDTO{

	// The ID of the target project.
	projectID: string = "";
	// The ID of the user who has or will have access to the project.
	accountID: string = "";
	// What the user can do with the project.
	permission: number = ProjectPermission.Viewer;
	// The ID of the user permission. Only used when editing the
	// property.
	permissionID: string = "";
}
