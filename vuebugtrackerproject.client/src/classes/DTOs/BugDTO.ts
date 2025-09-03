import Severity from "@/enumConsts/Severity";

export default class BugDTO{

	// A short description of the bug.
	summary: string = "";
	// The severity of the bug.
	severity: number = Severity.Low;
	// A longer, more detailed description of the bug.
	description: string = "";
	// The bug's ID. Only used when making changes to the project.
	bugID: string = "";
	// The ID of the project the bug belongs to.
	projectID: string = "";
  // Determines whether the bug is open or closed.
  isOpen: boolean = false;
}
