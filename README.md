GLITCH — Adaptive Game AI Under Runtime Instability

A Unity 6 third-person shooter built as a controlled experiment: does Utility AI hold up better than a Finite State Machine when the game world itself starts breaking?

MSc Software Engineering dissertation, University of Hertfordshire (module 7COM1039), supervised by Manjinder Kaur.



The question

Finite State Machines are still the default for enemy AI in shipped games. They are cheap, readable and predictable. Utility AI costs more to build and more to tune, and the usual argument for it is that it degrades more gracefully when conditions change.

That argument is rarely tested under conditions that actually change. GLITCH tests it by breaking the world on purpose and measuring what each architecture does next.

The design

Thirty enemy agents per session — fifteen Finite State Machine, fifteen Utility AI. Every agent shares the same body: identical movement, identical weapons, identical perception, identical health. The only difference between the two groups is how an agent decides what to do next. Anything measured afterwards is attributable to the decision architecture and nothing else.

Five scripted glitch events fire mid-match to destabilise the environment:







Event



What it does





Gravity flip



Inverts world gravity





Wall disappearance



Removes level geometry agents were using for cover and pathing





Enemy duplication



Spawns copies of existing agents





Time stutter



Disrupts the flow of time the agents act within





Player duplication



Presents multiple player targets at once



The measurements

A C# telemetry logger writes 13 behavioural fields per agent per second to CSV — state, action chosen, position, target, health and the rest — so every session produces a second-by-second record of what each agent decided and under what conditions.

Across 13 experiment sessions this produced 390 agent-runs, analysed in Python.

The result

Utility AI agents deployed a significantly wider behavioural repertoire than the FSM baseline. Where FSM agents collapsed toward a narrow set of states once their environmental assumptions were broken, Utility AI agents continued to select across a broader range of actions under the same conditions.



TODO before you publish this: replace this line with the two or three headline figures from Chapter 5 of the report — e.g. mean distinct actions per run for each group, and the significance test result. The claim above is true but a reader will trust a number more than an adjective.



A defect worth documenting

An early analysis round produced results that flattered the FSM baseline. The cause was a defect in my own utility scoring: the patrol action was missing from the action set entirely, and the attack action was not properly gated behind a line-of-sight check. Together these had biased agent decisions across the whole dataset.

I fixed both and re-ran the full experiment set rather than reporting the tainted numbers. The results above come from the corrected run.

Repository structure

Assets/           Unity project — scenes, agent scripts, telemetry logger, glitch event system
Packages/         Unity package manifest
ProjectSettings/  Unity 6 engine configuration
SessionLogs/      Raw CSV telemetry output from experiment sessions



Running it

Built with Unity 6.0. Clone the repository and open the project folder in Unity Hub with a Unity 6.0 editor installed; Unity will resolve packages on first open. Open the main scene in Assets/ and press Play to run a session. Telemetry is written to SessionLogs/ as CSV.

Built with

C# · Unity 6 · Python (pandas) for analysis

Author

Utsav Jivani — LinkedIn · Portfolio · utsavjivani.07@gmail.com
