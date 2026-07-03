function CriticalSystemError(message) {
  ///<summary>The error thrown when the given function isn't implemented.</summary>
  const sender = new Error().stack.split("\n")[2].replace(" at ", "");
  this.message = `The method ${sender} caused a crytical system error.`;

  // Append the message if given.
  if (message) this.message += ` Additional messsage: "${message}".`;

  let str = this.message;

  while (str.indexOf("  ") > -1) {
    str = str.replace("  ", " ");
  }

  this.message = str;
}

export default CriticalSystemError;
