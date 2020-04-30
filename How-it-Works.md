# How it Works

## Summary
This document explains some of the details about how the Poker Companion application works.

## Table of Contents

 1. [How Data is Interpreted](#how-data-is-interpreted)
 2. [Program Flow](#program-flow)
 3. [Database Structure](#database-structure)

## How Data is Interpreted
The Poker Companion application gets it's information by processing data in specific locations on the screen. Figure 1 shows a visualization of the bitmaps and pixels that are examined to determine the state of the game. This visualization can be seen by clicking the "Draw All Rectangles" button inside the "Bitmap Location Tools" groupbox inside the Control window.

 #### *Figure  1 - Data Used for Processing Game State*![enter image description here](https://adenyangos.github.io/GitHub-Pages_Test/images/Screen_Capture.PNG)

The largest white rectangle in Figure 1 is used to determine which player is the dealer and whose turn it is to act. This is done by checking the color of specific pixels in the locations where the gold dealer chip could be and the color of specific pixels in the locations where the white bars under the players appear when it is their turn to act. These locations are represented by small red rectangles in Figure 1, two for each player, one for the location where the dealer chip would be if that player was the dealer and one for the location where the white bar would be if it was that player's turn to act. In Figure 1 Quarantine is the dealer and JabaAdam is the player whose turn it is to act.

The white rectangle around the "Pot" text near the middle of the table is used to determine how many chips are in the pot. A bitmap image of this rectangle is converted to text and the value of the pot is extracted from that text.

The white rectangles on the board cards are used to determine the value of the board. First the application checks to see if a board card is present by checking the color of the pixels inside the small green rectangle in the top left corner of those white rectangles. If a card is present that pixel is also used to determine the suit of the card and the bitmap image inside the white rectangle is converted to text and the value of the card is extracted from that text. 

Figure 2 shows a close-up of the visualization of the bitmaps and pixels that are examined to determine the information about each player in the game. Figure 2 shows close-ups for two players with different hand conditions to help illustrate the process of determining each player's state.

 #### *Figure  2 - Player Data Used for Player State Close-ups*![enter image description here](https://adenyangos.github.io/GitHub-Pages_Test/images/Player_Info_Capture.PNG)

To asses the state of the player's hold cards the pixel colors inside the small red and green rectangles on the edges of the hold cards for player robert1123 are used to check if the player's cards are face down. If these pixels are white the card is face down. If these pixels are not white the color of the pixels inside the small red and blue rectangles inside of the heart and spade for player JabaAdam's hold cards are checked. If these pixels are white then the card is face up. If the card is face up the bitmaps inside the red and blue rectangles over the card's values are converted into text and the values are extracted from that text. The suit of face up hold cards is determined by evaluating the color of the pixels inside the small green rectangles at the top right of the card value rectangles. 

The pixel color inside the red rectangle between the the player's username and the player's chip count is checked to determine if there is currently a player in that seat. If there is no player there the text "Open Seat" will be shown and the the color inside the red rectangle will be light grey. Next the pixel color inside the red rectangle below the player's chip count is checked. If the player is all-in a banner covering the player's username and chip count will appear here showing the player's odds of winning.

If there is a player in the seat and no banner covering the player's information the player's username is determined by converting a bitmap with the player's username to text. This rectangle is not shown in Figure 2 but can be seen in Figure 3. The location of this rectangle changes depending on if the player has an avatar (picture) next to their name. The pixel color inside the small green rectangle inside the player's grey box is checked to determine if the player has an avatar next to their name. 

Figure 3 shows some of the bitmaps that were converted to text in order to process the data shown in Figures 1 and 2.

 #### *Figure  3 - Bitmaps Converted to Text*
![enter image description here](https://adenyangos.github.io/GitHub-Pages_Test/images/Processed_Bitmaps.PNG)

As seen in figure 3 the bitmaps that are converted to text are pre-processed to remove colors leaving black and white bitmap images to be converted to text.

Note that for the data to be processed correctly the BetOnline poker window must be maximized and the poker table must be a nine seat table because the bitmap files that are processed are found by using their absolute location on the screen. If the BetOnline window is not maximized or if it is not a nine player poker table the bitmaps taken will be in the wrong locations and as a result the data will not be interpreted correctly.

## Program Flow
This section will provide a high level overview of the program flow for processing screenshots.

After a screenshot of the BetOnline poker table is taken in live game mode or when a previously taken screenshot is loaded in screenshots mode the program flow proceeds as follows:

 - The large bitmap used to find the dealer and action player (shown in Figure 1) is copied from the BetOnline window screenshot and passed to a worker thread and that worker thread is started in order to determine the dealer and action player.
 - Next the bitmap for determining the pot size (shown in Figure 1) is copied from the BetOnline window screenshot and passed to a worker thread and that thread is started to determine the pot size.
 - Next the bitmaps to determine the status of each player and the five board cards (shown in Figure 1) are copied. If the worker thread that determines the dealer and action player has finished it's work the player or board card bitmap will be passed to a worker thread and that worker thread will be started to determine the relevant information.  If the worker thread that determines the dealer and action player has not finished it's work the UI  thread will move on and copy the next bitmap without launching the worker thread for the bitmap just copied. This is done because the work that the worker threads do to determine the status of each player and the five board cards depends on the data that the worker thread in charge of finding the dealer and action player produces. Essentially if the dealer has changed this is a new hand and everything is checked, but if not this is not a new hand so players that have already folded and board cards that have already been processed don't need to be re-checked. 
 - After the above process has been completed for each player and the five board cards the application goes back to determine which worker threads were never started because they were waiting for the results of the worker that determines if we have a new dealer and starts those worker threads. 
 - Next the application waits for all the workers to finish their work. Then the UI thread checks for a special condition where there is a new hand but the dealer does not change (I won't get into the details of what causes this situation but it does happen sometimes). If this special condition has occurred the worker threads for all the players and the board cards are re-run under the conditions of a new hand.
 - Next all the game data is updated on the UI. The UI will announce new hands, any board cards or hold cards that were shown, any folds and any changes in the player's chip stacks. This information will be saved to the database.
 - If the player of interest has just become the action player the Data Display window will be updated. Relevant data for each player at the table is queried from the database. The player chip stack change and board card change data is used to convert player chip stack changes into checks, calls, and bets which are displayed in the Data Display window. 

After all the above processes are complete, if in live game mode, another screenshot is taken and the process is re-started. If in screenshots mode the application waits for the next screenshot to be passed to it.

## Database Structure
The structure of the database that stores the processed data is shown in Figure 4.

 #### *Figure  4 - Database Structure*![enter image description here](https://adenyangos.github.io/GitHub-Pages_Test/images/Database_Structure.PNG)

This database stores the data required to determine each player's folds, checks, calls and bets in each hand, all the hands certain players had with other players and how many players were in each of those hands. In addition to that information, which is the information this application presents to the user, the data in this database also contains the information necessary to do the following:

 - Differentiate between bets, raises and re-raises.
 - Identify all-ins.
 - Determine more complex information about how players play such as:
	 - If they play differently based on their betting position (the player's relationship to the dealer button).
	 - How the players play with respect to the strength of their hands (based on data from hands where they show their cards).
	 - How the players respond to larger bets vs. smaller bets.
	 - How the players play when they are short stacked vs when they aren't. 
 - Much more.