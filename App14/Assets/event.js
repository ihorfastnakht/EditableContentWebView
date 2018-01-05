(function () {
    function showMsg(message) {
        window.external.notify(message);
    }

    if (typeof window.catchEvents !== "function") {


        function catchEvents() {
            window.addEventListener("keydown", keydown, false);
        }

        function keydown(e) {
            var event = {
                Type: "__KEYBOARD_EVENT",
                Error: "",
                Message: {
                    Ctrl: e.ctrlKey,
                    Alt: e.altKey,
                    Shift: e.shiftKey,
                    Char: e.char,
                    Key: e.key,
                    KeyCode: e.keyCode,
                    Type: e.type
                }
            }
            showMsg(JSON.stringify(event));
        }

        function mouseDown(e) {
            var event = {
                Type: "__MOUSE_EVENT",
                Error: "",
                Message: {
                    Ctrl: e.ctrlKey,
                    Alt: e.altKey,
                    Shift: e.shiftKey,
                    Button: e.buttons,
                    Type: e.type
                }
            }
            showMsg(JSON.stringify(event));
        }

        window.catchEvents = catchEvents;
    }

    window.catchEvents();

})()