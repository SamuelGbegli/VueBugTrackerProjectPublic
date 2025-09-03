export default class BugPreviewViewModel{

	// Unique identifier for the bug.
	id: string = "";
	// A short description of the bug.
	summary: string = "";
	// The severity of the bug.
	severity: string = "";
	// The ID of the user that created the bug.
	creatorID: string = "";
	// The name of the user that created the bug.
	creatorName: string = "";
	// The icon of the user that created the bug.
	creatorIcon: string = "";
  // The status of the bug, determining whether it is open or closed.
  status: string = "";
	// The number of comments and status updates the bug has.
	numberOfComments: number = 0;
	// The date the bug was last updated.
	dateModified: Date = new Date();
}
