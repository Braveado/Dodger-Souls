var BrowserCom =
{

  SendName1: function ()
  {
    SendMessage("P1", "RecieveName", document.getElementById("name").textContent);
  },

  SendAvatar1: function ()
  {
    SendMessage("P1", "RecieveAvatar", document.getElementById("avatar").textContent);
  },

  SendName2: function ()
  {
    SendMessage("P2", "RecieveName", document.getElementById("name").textContent);
  },

  SendAvatar2: function ()
  {
    SendMessage("P2", "RecieveAvatar", document.getElementById("avatar").textContent);
  },

  GetScore: function (str)
  {
    document.getElementById("score").textContent = Pointer_stringify(str);
  }

};
mergeInto(LibraryManager.library, BrowserCom);