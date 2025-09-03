//View model for showing a preview of a project.
import AccountViewModel from "./AccountViewModel";

export default class ProjectPreviewViewModel{

	// Unique identifier for the project.
	id: string = "";
	// The name of the project.
	name: string = "";
	// The short description of the project.
	summary: string = "";
	// The ID of the user that created the project.
	ownerID: string =  "";
	// The name of the user that created the project.
	ownerName: string =  "";
	// The icon of the user that created the project.
	ownerIcon: string =  "";
	// The number of bugs in the project are open.
	openBugs: number = 0;
	// The total number of bugs in the project.
	totalBugs: number = 0;
	// The date the project was last updated.
	dateModified: Date;

}
