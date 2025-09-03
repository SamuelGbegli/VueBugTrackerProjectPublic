import type ProjectPreviewViewModel from "./ProjectPreviewViewModel";

// Container for returning a set of projects from the server.
export default class ProjectController{
  // The number of projects the user can see.
  totalProjects: number = 0;
  // The projects the user can view.
  projects: ProjectPreviewViewModel[] = [];
}
