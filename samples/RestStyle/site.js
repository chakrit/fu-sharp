$(function() {

  function createNoteItem(note) {
    var item = $(document.createElement("li"));
    var delLink = $(document.createElement("button"));

    delLink.click(function() {
      $.ajax({
        type: "DELETE",
        url: "/notes/" + note,
        success: function() { item.remove() }
      });
    });

    delLink.text("del");
    item.text(note + " - ")
      .append(delLink)
      .appendTo($("#notes"));
  }

  // forms only support GET and POST so we'll need to use Ajax calls
  $("#newNoteForm").submit(function() {
    $.ajax({
      type: "PUT",
      url: "/notes",
      dataType: "json",
      data: { note: $("textarea", this).val() },
      success: createNoteItem
    });

    return false;
  });

  // load existing list of notes
  $.ajax({
    type: "GET",
    url: "/notes",
    dataType: "json",
    success: function(data) {
      $.map(data, createNoteItem);
    }
  });

});
