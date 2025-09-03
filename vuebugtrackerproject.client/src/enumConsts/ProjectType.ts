// Descibes the types of projects being search for.
const ProjectType = {
  //Gets all projects
  All: 0,
  //Only gets projects with no bugs that are open
  NoOpenBugs: 1,
  //Only gets projects with at least one open bug
  OpenBugsOnly: 2
} as const;

export default ProjectType;
