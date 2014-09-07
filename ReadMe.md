# OnCallSchedulerAssistant
Helps in creating an on call schedule. It takes into account days where an agent cannot work, and assigns various point values to days, ie. week-ends are worth twice as much as week days.
# Usage
ocsa [AgentsFile] [StartDate]
By default, AgentsFile is "./Agents.txt" and StartDate is now. The schedule is made for a period of 28 days. The schedule is written in "./Schedule.txt".