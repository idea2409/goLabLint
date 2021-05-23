// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function myFunction() {
    document.getElementById("myDropdown").classList.toggle('show');
  }

  window.onclick = function(event) {
    if (!event.target.matches('img.user-profile')) {
      var dropdowns = document.getElementsByClassName("user-dropcontent");
      var i;
      for (i = 0; i < dropdowns.length; i++) {
        var openDropdown = dropdowns[i];
        if (openDropdown.classList.contains('show')) {
          openDropdown.classList.remove('show');
        }
      }
    }
  }