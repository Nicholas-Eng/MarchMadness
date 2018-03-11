# MarchMadness
:basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball:

:basketball::basketball::basketball::basketball: Sila Solutions Group :basketball::basketball::basketball::basketball:

:basketball::basketball::basketball::basketball: March Madness 2018 :basketball::basketball::basketball::basketball:

:basketball::basketball::basketball::basketball::basketball:  Nicholas Eng :basketball::basketball::basketball::basketball::basketball:

:basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball::basketball:

## Description
This is a console application built in C# and .NET Core.

## How to Run Application
There are two ways to run the application after pulling down the project.

1. Run inside IDE.
2. Run the executable: "MarchMadness\bin\Release\netcoreapp2.0\win10-x64\MarchMadness.exe".

## How it Works
1. User enters two teams.
2. Data for both teams are read based off the seasons listed in the Constants.cs file.
3. The retrieved data is stored and, in some cases, manipulated into objects.
   - For example, collecting averages of data.
4. A running point system is calculated for each category of data (i.e. Average Steals) for every season.
   - The point system looks at which team had the better stat, then in some cases takes the difference divided by a pre-determined typical spread value.
5. Once all the points for each category in every season are collected, the sum is then calculated for all points for each team in every season.
6. Then, based off the number of seasons, the sum for that season is multiplied by the exponent of the season relative to this year.
   - For example, if our totals for 2015 = 100, 2016 = 200, 2017 = 300 then our equations would look like:
     (100 * e^(1)) + (200 * e^(2)) + (300 * e^(3)) = final score
   - The exponent is used so that the more recent seasons are weighted heavier than older seasons.
7. The summation of each value calculated in step 6 then gives us the team's final score.
8. The team with the higher final score is deemed the winner.
