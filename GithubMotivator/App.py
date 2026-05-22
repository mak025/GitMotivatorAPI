from flask import Flask, request, jsonify
from flask_cors import CORS
from sense_hat import SenseHat
import threading
import time

app = Flask(__name__)
CORS(app)

sense = SenseHat()
sense.low_light = True
sense.set_rotation(0)


# Global variable to hold the current message
current_message = "Hej"
stop_thread = False
GREEN = [0, 255, 0]
YELLOW = [255, 255, 0]
RED = [255, 0, 0]
BLUE = [0, 0, 255]
OFF = [0, 0, 0]

def display_message():
    global current_message, stop_thread
    while not stop_thread:
        sense.show_message(current_message, scroll_speed=0.1, text_colour=[0, 10, 255], back_colour=[0, 0, 0])

def celebration(message="MILESTONE!"):
    colors = [GREEN, YELLOW, RED, BLUE]

    for i in range(6):
        sense.clear(colors[i % len(colors)])
        sleep(0.2)
        sense.clear(OFF)
        sleep(0.1)

    sense.show_message(
        message,
        scroll_speed=0.05,
        text_colour=GREEN,
        back_colour=OFF
    )
    sleep(5)
    sense.clear()

# Start the message display thread
thread = threading.Thread(target=display_message)
thread.daemon = True  # This makes sure the thread will exit when the main program does
thread.start()

@app.route("/")
def home():
    return "Sense HAT backend kÃ¸rer"

@app.route("/message", methods=["POST"])
def message():
    global current_message
    data = request.json
    current_message = data.get("text", "Hej")
    return jsonify({"ok": True, "message": current_message})

@app.route("/clear", methods=["POST"])
def clear():
    global current_message
    current_message = ""  # Clear the message
    sense.clear()
    return jsonify({"ok": True})

@app.route("/stop", methods=["POST"])
def stop():
    global stop_thread
    stop_thread = True  # Stop the display thread
    sense.clear()
    return jsonify({"ok": True})

@app.route("/celebration", methods=["POST"])
def trigger_celebration():
    data = request.get_json() or {}
    message = data.get("message", "MILESTONE!")

    celebration(message)

    return jsonify({
        "status": "ok",
        "message": message
    })

app.run(host="0.0.0.0", port=5000)