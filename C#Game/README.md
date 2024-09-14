# Assignment 2

Game: Apple Rush

Game Play:


https://github.com/user-attachments/assets/13d18fc3-3426-45ff-aa9b-902f25f5af88



Introduction: There are apples falling from the sky. You have a basket to collect has many as possible in 120 seconds!

How to Play: Use left arrow and right arrow to move the basket. In order to catch an apple, you need to place it below the apple. The rule is displayed before the game starts. You'll have 120 seconds. During the game, your current score and remaining time is on the screen. When time's up, you will see your score and you can click to play again.

How to Build and Run:
	
 	csc *.cs
	.\Window.exe

Design:

The game consists of three main classes: Apple.cs, Basket.cs, and Game.cs, also the Window.cs file that sets up the game window.

Game.cs: The Game object is responsible for managing the overall game state and apples and the basket. During each frame, the game loop calls the Update(float dt) method in the Game class. Once the game starts, it updates the position of each apple by calling the Update(float dt) method in the Apple class. The Game object also checks if an apple has been caught by the basket using the isCaught(Basket basket) method, and if it has, it increases the score and resets the apple’s position. Similarly, if an apple falls out of bounds, it is reset to the top. Additionally, the Basket object is updated based on player input through the KeyDown() method, which moves the basket left or right. The game tracks time, ensuring that the game lasts for 120 seconds, and after that, it displays the final score and allows the player to restart.

Apple.cs: Each Apple object manages its own movement and interaction with the basket. The Update(float dt) method is called every frame to move the apple downward based on its speed and the time delta (dt). The position of the apple is updated incrementally, 		simulating its fall from the top of the window. The Apple object also includes logic to check if it has been caught.

Basket.cs: The Basket object represents the player’s control in the game, and it is updated based on player input. When the player presses the left or right arrow keys (or A and D), the KeyDown(KeyEventArgs key) method in the Game class calls the MoveLeft(float 		dt) or MoveRight(float dt) methods in the Basket class. These methods update the basket’s position by adjusting its x-coordinate based on the basket's speed and the delta time (dt). The basket’s movement is restricted within the window boundaries, ensuring it 		doesn't move off screen. The basket constantly checks for apples within its range, and if an apple's position overlaps with the basket's, it counts as caught.

Window.cs: sets up the game window and handles the game loop
