//Assigns chip colour to bug severity and status fields
export default function getChipColour(value: string){

  switch(value){
    case "Low":
      return "blue";
    case "Medium":
      return "amber";
    case "High":
    case "Closed":
      return "red";
    case "Open":
      return "green";
  }
}
