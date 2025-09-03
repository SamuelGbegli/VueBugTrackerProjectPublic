import Severity from "@/enumConsts/Severity";

export default class BugViewModel{

	// Unique identifier for the bug.
	id: string = "";
	// A short description of the bug.
	summary: string = "";
	// The severity of the bug.
  severity: number = Severity.Low;
	// IIf true, means the bug is yet to be resolved.
  isOpen: boolean = false;
	// A longer, more detailed description of the bug.
	description: string = "";
	// The ID of the user that created the bug.
	creatorID: string = "";
	// The name of the user that created the bug.
	creatorName: string = "";
	// The icon of the user that created the bug.
	creatorIcon: string = "";
	// The number of comments and status updates the bug has.
	numberOfComments: number = 0;
	// The date the bug was created.
	dateCreated: Date = new Date();
	// The date the bug was last updated.
	dateModified: Date = new Date();
	// The ID of the project the bug belongs to.
	projectID: string = "";
	// The name of the project the bug belongs to.
	projectName: string = "";
	// The ID of owner of the project the bug belongs to.
	projectOwnerID: string = "";
}
