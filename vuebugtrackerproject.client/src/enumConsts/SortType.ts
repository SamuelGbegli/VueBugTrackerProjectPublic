// Used to decide what to sort projects on.
const SortType ={
  // Sorts projects by their name
  Name: 0,
  // Sorts projects by when they were created.
  CreatedDate: 1,
  // Sorts projects by when they were last updated.
  LastUpdated: 2
} as const;

export default SortType;
