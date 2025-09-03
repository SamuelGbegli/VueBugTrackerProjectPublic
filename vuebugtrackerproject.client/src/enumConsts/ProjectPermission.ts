//Describes a role a user has with the project.
const ProjectPermission = {
  //Grants the user permission to view a project. Only used if the project is restricted.
  Viewer: 0,
  //Allows the user to add and modify bugs in a project.
  Editor: 1,
  //Indicates the user owns the project. This is only assigned when the user creates the project.
  Owner: 2
} as const;

export default ProjectPermission;
