# **DOM totals lines**
This is a resume of the DOM.\
You need Data II level to get the level values.\
I use this indicator as a filter when i have a trading setup.

It plots:
* Two lines, so i can see the history of the DOM across my chart\
  Green: are the Asks from the DOM (Sell limit orders)\
  Red: are the Bids of the DOM (Buy limit orders)  
* A small rectangle by default in the upper right corner with the same data.
  
  
Parameters:
* Level count: the number of DOM levels to consider the totals
* Custom tick size: Value required by **GetLevel2ItemsParameters** class
* Font size: for the values inside the rectangle
* Paint rectangle: if true it plots the rectangle on the chart, if false the rectangle is off
* X & Y coordinates for the rectangle

![image](https://user-images.githubusercontent.com/69223009/133335583-dbecb6d4-327b-4b20-a611-756a3498b110.png)

## Example

![dom lines](https://user-images.githubusercontent.com/69223009/133333903-10327818-e90d-4910-9d22-7f24cbb2c0e4.png)

As you can see in the previous image the indicator has a delay vs the original DOM, this is because the chart updates the values each tick and the DOM moves faster (100ms by default). For my use, this doesnt matter because i use it only as a filter.

This indicator lines does not follow the normal logic. 
