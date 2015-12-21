# RandCityGenGamesEnginesAsignment
GameEngines1 Assignment

Controls:
Press play in Unity

The game window will appear in first  person mode

To change to first person controller movement press 1

Press 9 twice to load a new city the first time and for the remaining times press it once.
Double pressing 9 is a feature (bug) that came with last minute addition of time speed change that was recently added.

changing the parameters...
r  = radius
pV = perimeter variation
pdmin = point distance min 
pdmax = "            " max

will produce different citiy shapes

other important variables include 
-skySkraperratio 
-building ratio

this is out of 100
default values should be 
-skySkraperratio 5
-buildingRatios 50
remaining 45 wil be parks

other default Values 
r     = 500
pdmin = 10
pdmax = 70
pV    = 100

The Voronoi digram comes from https://github.com/jceipek/Unity-delaunay
