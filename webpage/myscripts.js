var oldMessage, mostRecentMessage;

function Connect() {
   username = document.getElementById("username").value;
   window.location = "server.html";
}

function SendMsg(ele) {
   let username = "Owen";
   if(event.key === 'Enter' || event.button === 0) {
      const msg = document.getElementById("text").value;
      const please = document.querySelector('.message');

      document.getElementById("text").value = "";

      var newMessage = document.createElement("li");
      newMessage.innerHTML = `
      <li id="chatbox" style="margin-bottom:15px; font-size: 125%;" class="most-recent">
         <div class="usernameAndMsg">
            <p class="user">${username}</p>
            <p id="most-recent-message">${msg}</p>
         </div>
         <img class="sent" src="images/user-icon.png" width="40px">
      </li>
      `
      message.append(newMessage);
   }
}

function receiveMsg() {
   msg = "This is a test..."
   username = "AI"
   var newMessage = document.createElement("li");
   newMessage.innerHTML = `
   <li id="chatbox" style="margin-bottom:15px; font-size: 125%;" class="r-most-recent">
      <img class="r-sent" src="images/user-icon.png" width="40px">
      <div class="r-usernameAndMsg">
         <p class="user">${/*sentUsername*/username}</p>
         <p id="most-recent-message">${msg}</p>
      </div>
   </li>
   `

   message.append(newMessage);
   storeText(msg);
}

function storeText(message) {
   mostRecentMessage = oldMessage
   message = mostRecentMessage
}