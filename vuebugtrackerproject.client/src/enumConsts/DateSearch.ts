// Used to determine whether to sort a project by when it was created
// or last updated.
const DateSearch ={
  //Filters projects by when they were created.
  CreatedDate: 0,
  // Filters projects by when they were last updated.
  LastUpdated: 1
} as const;

export default DateSearch;
