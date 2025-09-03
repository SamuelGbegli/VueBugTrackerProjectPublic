//View model for a project.

import Visibility from "@/enumConsts/Visibility";
import AccountViewModel from "./AccountViewModel";

export default class ProjectViewModel{

	// Unique identifier for the project.
	id: string = "";
	// The name of the project.
	name: string = "";
	// The short description of the project.
	summary: string = "";
	// The project's external link.
	link: string = "";
  // The project's visibility.
  visibility: number = Visibility.Public;
	// The ID of the user that created the project.
	ownerID: string =  "";
	// The name of the user that created the project.
	ownerName: string =  "";
	// The icon of the user that created the project.
	ownerIcon: string =  "";
	// The number of bugs in the project are open.
	openBugs: number = 0;
	// The total number of bugs in the project.
	totalBugs: number  = 0;
	// The date the project was created.
	dateCreated: Date = new Date();
	// The date the project was last updated.
	dateModified: Date = new Date();
	// The formatted project description.
	description: string = "";
	// The project's tags.
	tags: string[] = [];
}
