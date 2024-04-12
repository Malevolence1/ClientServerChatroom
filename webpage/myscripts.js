let username = "";

function Connect() {
   username = document.getElementById("username").value;
   window.location = "server.html";
}

function SendMsg(ele) {
   if(event.key === 'Enter' || event.button === 0) {
      const msg = document.getElementById("text").value;
      const please = document.querySelector('.message');

      document.getElementById("text").value = "";

      var newMessage = document.createElement("li");
      newMessage.innerHTML = `
      <li id="chatbox" style="margin-bottom:15px; font-size: 125%;" class="most-recent">
         <p id="most-recent-message">${msg}</p>
         <img class="sent" src="images/user-icon.png" width="40px">
      </li>
      `
      message.appendChild(newMessage);
      console.log(username);
   }
}

function receiveMsg() {
   msg = "This is a test..."
   var newMessage = document.createElement("li");
   newMessage.innerHTML = `
   <li id="chatbox" style="margin-bottom:15px; font-size: 125%;" class="r-most-recent">
      <img class="r-sent" src="images/user-icon.png" width="40px">
      <p id="most-recent-message">${msg}</p>
   </li>
   `

   message.appendChild(newMessage);
}