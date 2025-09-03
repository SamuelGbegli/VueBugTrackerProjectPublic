import Visibility from "@/enumConsts/Visibility";

export default class ProjectDTO{

	// The project's name.
	Name: string = "";
	// The short description of the project.
	Summary: string = "";
	// A link to the project's site.
	Link: string = "";
	// Determines who can see the project.
	Visibility: number = Visibility.Public;
	// The raw, unformatted long description of the project.
	Description: string = "";
	// The long description of the project, with HTML tags to style
	// the text.
	FormattedDescription: string = "";
	// The project's tags.
	Tags: string[] = [];
	// The project's ID. Only used when making changes to the project.
	ProjectID: string = "";
}
