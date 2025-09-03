//Removes HTML tags from user input. Based on this site:
//https://www.30secondsofcode.org/js/s/strip-html-tags/
export default function removeHTMLTags(input: string){
  return input.replace(/<[^>]*>/g, "");
}
