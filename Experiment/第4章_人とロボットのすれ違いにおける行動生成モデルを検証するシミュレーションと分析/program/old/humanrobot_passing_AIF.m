% This shows "two agents' passing behavior with AIF model. 
% 
% Agent: Robot and Human
% Generative model: independent(policy inference about only itself)
%                   to be revaised mutual policy inference
%
% 
% Robot and human pass thorough each other with predicting outer world
% using their "independent" generative model.


% ------------------------------------------------------------------------
% Initialize MATLAB workspace.
% ------------------------------------------------------------------------

clear all
close all      % These commands clear the workspace and close any figures

rng('shuffle') % This sets the random number generator to produce a different 
               % random sequence each time, which leads to variability in
               % repeated simulation results (you can alse set to 'default'
               % to produce the same random sequence each time).


% ------------------------------------------------------------------------
% Determine simulation setting.
% ------------------------------------------------------------------------

Sim = 0;      % Simulation number.
              % With this index, it can be conducted with different conditions.


trialN = 5;   % Trial time in all simulation.

t = 5;        % Considerd time point in the model.

T = 2;        % Simulated time step at each reflection to generative process from anothor agent model.
              % IF T = 2, agents do something for 1 step and observe outer
              % changes that is resulted from both itseld and other's action.

Tact = 5;     % Simulated sum of time points.
 


%% MAP CREATER %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% ------------------------------------------------------------------------
% Determine the conditions of a map, that is a road where agents move. 
% ------------------------------------------------------------------------


% Consider the size of the road.

MAPw = 6;     % Width(vertical line) of the road
MAPl = 8;     % Length(horizontal line) of the road

MAPwr = 3;    % Start location of ROBOT regarding width line (>= 2).
MAPwh = 3;    % Start location of HUMAN regarding width line (>= 2).

MAP = zeros(MAPw,MAPl);     % CREATING MAP, that is the road for agents passing thorough.

Ns = numel(MAP);    % "Number of position(states)" in the road.

STARTr = sub2ind(size(MAP),MAPwr,1);       % ROBOT's initial position.
STARTh = sub2ind(size(MAP),MAPwh,MAPl);    % HUMAN's initial position.


% Consider locations where a wall exists. 

Wrisk = 10;             % This risk index related with the prefrerence regarding the wall collision
MAP(1,:) = Wrisk;       % It indicates "Wall"(upper).
MAP(MAPw,:) = Wrisk;    % It indicates "Wall"(lower).


%% SUBJECTIVE PROBABILITY DISTRIBUTION %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% ------------------------------------------------------------------------
% Calculate the risk against other agent, that is directly related with
% the preference regarding collision avoidance.
% It takes a little longer time so doing at initial phase.
% ------------------------------------------------------------------------


% Consider the parameters among agent's prediction.

maxsp    = 8;         % Max speed in prediction for the other.
musp     = 2;         % Average speed in prediction for the other.
sigmasp  = 1;         % Uncertainty of speed in prediction for the other

mudir    = 0;         % Average direction in prediction for the other
sigmadir = pi/16;     % Uncertainty of direction in prediction for the other

SIZErh   = 2;         % Related size of two agents.


% Calculate the distribition of potantial risks using the parameters.

x = -(MAPl-1):(MAPl-1);     % Horizontal axis for mesh.
y = -(MAPw-1):(MAPw-1);     % Vertical axis for mesh.

[X,Y] = meshgrid(x,y);
R = sqrt(X.^2 + Y.^2);
[theta,rho] = cart2pol(X,Y);    

Z = zeros(size(R));     % Matrix[(MAPw*2-1)*(MAPl*2-1)]
W = zeros(size(R));     % Considered agent's size

syms r
f = r*r*normpdf(r,musp,sigmasp);    % Squared velocity with probability density.

G = normpdf(theta,mudir,sigmadir);  % Provbability density about direction.

M = size(R,1);  % Vertical length.
N = size(R,2);  % Horizon length.


% "Z" is shown as positive quantity(size of risk).

parfor i = 1:M
    for j = 1:N

        Z(i,j) = int(f,[R(i,j),maxsp])*G(i,j);

    end
end


for i = 1:N
    for j = 1:M

        P = X(1,i);
        Q = Y(j,1);

        for k = 1:N
            for l = 1:M

                p = X(1,k);
                q = Y(l,1);
               
                if (P-p)^2+(Q-q)^2 < SIZErh^2
                    W(l,k) = W(l,k) + Z(j,i);
                else

                end

            end
        end

    end
end


% ------------------------------------------------------------------------
% IF display this culclated rusult of risk index at each postion, 
% You can see the potential risks regarding other agnet collision.
% ------------------------------------------------------------------------


%% GENERATIVE MODEL STRUCTURE %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% ------------------------------------------------------------------------
% Label for states, outcomes, and actions for subsequent plotting.
% It's not entirely unnecessary for conducting the simulation, but
% You can use this label for showing results with its variable names. 
% ------------------------------------------------------------------------


label.factor{1}   = 'robot position';   % label.name{1}    = {'state 1','state 2'};
label.factor{2}   = 'human position';
label.modality{1} = 'robot location';   % label.outcome{1} = {'outcome 1','outcome 2'};
label.modality{2} = 'human location';
label.modality{3} = 'robot-human related location';
label.modality{4} = 'agent location(with wall)';
label.action{1} = {'up','down','right'};
label.action{2} = {'straight','above','below'};
mdpr.label = label;
mdph.label = label;


%% PRIOR %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Priors about initial states: D and d
% ========================================================================
% D{state factor}(state,1)


D{1} = zeros(Ns,1);
D{1}(STARTr,1) = 1;     % Robot starts at the start position.

D{2} = zeros(Ns,1);
D{2}(STARTh,1) = 1;     % Human starts at the start position.


%% LIKELIHOOD %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% State-outcome mappings and beliefs: A and a
% ========================================================================
% A{outcome modality}(outcome,factor1,factor2,...)


No = zeros(1,numel(label.modality));    % Number of outcome at each outcome modality.


% Modality1: Rboot location
% ------------------------------------------------------------------------

No(1) = Ns;
A{1} = zeros(Ns,Ns,Ns);     % Definite location of Robot.

Po = 1;     % Precision of outcome.

for k = 1:Ns
    A{1}(k,k,:) = Po;       % A{1}(Robot location, Robot postion, Human position)
end


% Modality2: Human location
% ------------------------------------------------------------------------

No(2) = Ns;
A{2} = zeros(Ns,Ns,Ns); % Definite location of human

Po = 1;     % Precision of outcome.

for k = 1:Ns
    A{2}(k,:,k) = Po;       % A{2}(human location, robot postion, human position)
end


Ar = A;     % Robot and Human observe both their lacation as well.
Ah = A;     % Therefor, regarding those tow modality, they have same observation mapping at this time.


% Modality3: Robot-Human relative location
% ------------------------------------------------------------------------

RELATIVE_LOCATION = zeros(MAPw*2-1,MAPl*2-1);
No(3) = numel(RELATIVE_LOCATION);

Ar{3} = zeros(No(3),Ns,Ns);      % Relative location outcome at each robot and human position.

for i = 1:Ns   % Robot position
    for j = 1:Ns    % Human position
        
        % We caluculate related location from human: robot infer colision
        % risk based on predicted human velocity: THAT IS, on related
        % location map, human centered, robot position.

        [rm,rn] = ind2sub(size(MAP),i);     % Robot location subscript[row,col]
        [hm,hn] = ind2sub(size(MAP),j);     % Human location subscript[row,col]
        RLm = hm - rm + MAPw;   % Vertical subscript(row)
        RLn = hn - rn + MAPl;   % Horizontal subsclipt(col)

        RLind = sub2ind(size(RELATIVE_LOCATION),RLm,RLn);    % Index on related location mapping.
        
        Ar{3}(RLind,i,j) = 1;    % Related location at robot and human's own position.

    end
end


Ah{3} = zeros(No(3),Ns,Ns);      % relative location outcome at each robot and human position

for i = 1:Ns   % Robot position
    for j = 1:Ns    % Human position
        
        % We caluculate related location from human: robot infer colision
        % risk based on predicted human velocity: THAT IS, on related
        % location map, human centered, robot position.

        [rm,rn] = ind2sub(size(MAP),i);     % Robot location subscript[row,col]
        [hm,hn] = ind2sub(size(MAP),j);     % Human location subscript[row,col]
        RLm = rm - hm + MAPw;   % Vertical subscript(row).
        RLn = rn - hn + MAPl;   % Horizontal subsclipt(col).

        RLind = sub2ind(size(RELATIVE_LOCATION),RLm,RLn);    % Index on related location mapping.
        
        Ah{3}(RLind,i,j) = 1;    % Related location at human and robot's own position.

    end
end


% Modality4-1: Agent location(considering wall) of Robot
% ------------------------------------------------------------------------

No(4) = Ns;
Ar{4} = zeros(Ns,Ns,Ns); % Definite location of robot

Po = 1;     % Precision of outcome

for k = 1:Ns
    Ar{4}(k,k,:) = Po;       % A{4}(Related location, Robot postion, Human position)
end



% Modality4-2: Agent location(considering wall) of Human
% ------------------------------------------------------------------------

No(4) = Ns;
Ah{4} = zeros(Ns,Ns,Ns); % Definite location of robot

Po = 1;     % Precision of outcome

for k = 1:Ns
    Ah{4}(k,:,k) = Po;       % A{4}(Related location, Robot postion, Human position)
end


%% STATE TRANSITION %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% allowable policies (1 move): U
%=========================================================================
% U(1,action number,state factor)

% ------------------------------------------------------------------------
% u  = [below; straight = right(robot)/left(human); above; others]
% ------------------------------------------------------------------------


ur   = [1 0; 0 1; -1 0];      % Allowable actions of Robot.
uh   = [1 0; 0 -1; -1 0];     % Allowable actions of Human.
nur  = size(ur,1);      % Number of robot actions.
nuh  = size(uh,1);      % Number of human actions.
nu   = nur;


% Policy in Robot generative model
% ------------------------------------------------------------------------

Ur = zeros(1,nu,numel(label.factor));       % Policy which Robot might have.

Ur(1,:,1) = 1:nur;      % Controlable regarding robot itself(1)
Ur(1,:,2) = 1:nuh;      % Uncontrolable regarding human(2)


% Policy in Human generative model
% ------------------------------------------------------------------------

Uh = zeros(1,nu,numel(label.factor));       % Policy which human might have.

Uh(:,:,1) = 1;          % Uncontrolable regarding robot(1).
Uh(1,:,2) = 1:nuh;      % Controlable regarding human itself(2).


% ------------------------------------------------------------------------
% It should be revised as "mutual polcy inference", that is,
% U inculudes all agent' actions.
% ------------------------------------------------------------------------



% Controlled transitions and transition beliefs : B{:,:,u} and b(:,:,u)
%=========================================================================
% B{state factor}(state at time tau+1,state at time tau,action number)

% ------------------------------------------------------------------------
% Transition mapping in Robot generative model.
% ------------------------------------------------------------------------


% Factor1: Robot position
% ------------------------------------------------------------------------

Ps = 1;     % Precision of state transition.

Br{1} = zeros(Ns,Ns,nur);     % Robot position transition.
[n,m] = size(MAP);
for i = 1:n
    for j = 1:m
        
        % Allowable transitions from state s to state ss.
        %-----------------------------------------------------------------
        s     = sub2ind([n,m],i,j);
        for k = 1:nur
            try
                ss = sub2ind([n,m],i + ur(k,1),j + ur(k,2));
                Br{1}(ss,s,k) = Ps;     % Move at the policy.                
            catch
                Br{1}(s ,s,k) = 1;
            end
        end
    end
end

br{1} = Br{1};


% Factor2: Human position(robot predict it)
% ------------------------------------------------------------------------

PPh = [0.1 0.8 0.1];     % Prediction precision of human position transition.
                         % [above; straight; below]

tempbr = zeros(Ns,Ns,1);

br{2} = zeros(Ns,Ns,nuh);     % Human position transition(uncontrolable)
[n,m] = size(MAP);
for i = 1:n
    for j = 1:m
        
        % Allowable transitions from state s to state ss.
        %-----------------------------------------------------------------
        s     = sub2ind([n,m],i,j);
        for k = 1:nuh
            try
                ss = sub2ind([n,m],i + uh(k,1),j + uh(k,2));
                tempbr(ss,s,1) = PPh(k);     % Move at the action.
            catch
                
            end
        end
        ssum = sum(br{2},[1 3]);
        tempbr(s,s,1) = 1-ssum(s);           % Rest posibility.
    end
end

for i = 1:nuh
    br{2}(:,:,i) = tempbr;
end

TPPh = [0 1 0];     % True precision of human position transition.

tempBr = zeros(Ns,Ns,1);

Br{2} = zeros(Ns,Ns,nuh);     % Human position transition(uncontrolable)
[n,m] = size(MAP);
for i = 1:n
    for j = 1:m
        
        % Allowable transitions from state s to state ss.
        %-----------------------------------------------------------------
        s     = sub2ind([n,m],i,j);
        for k = 1:nuh
            try
                ss = sub2ind([n,m],i + uh(k,1),j + uh(k,2));
                tempBr(ss,s,1) = TPPh(k);     % Move at the action.
            catch
                
            end
        end
        ssum = sum(Br{2},[1 3]);
        tempBr(s,s,1) = 1-ssum(s);            % Rest posibility.
    end
end

for i = 1:nuh
    Br{2}(:,:,i) = tempBr;
end



% ------------------------------------------------------------------------
% Transition mapping in Human generative model.
% ------------------------------------------------------------------------


% Factor1: Robot position(Human predict it).
% ------------------------------------------------------------------------

PPr = [0.1 0.8 0.1];     % Prediction precision of robot position transition.

bh{1} = zeros(Ns,Ns,1);     % Human position transition(uncontrolable).
[n,m] = size(MAP);
for i = 1:n
    for j = 1:m
        
        % Allowable transitions from state s to state ss.
        %-----------------------------------------------------------------
        s     = sub2ind([n,m],i,j);
        for k = 1:nur
            try
                ss = sub2ind([n,m],i + ur(k,1),j + ur(k,2));
                bh{1}(ss,s,1) = PPr(k);     % Move at the action.
            catch
                
            end
        end
        ssum = sum(bh{1},[1 3]);
        bh{1}(s,s,1) = 1-ssum(s);           % Rest posibility.
    end
end

TPPr = [0 1 0];     % True precision of robot position transition.

Bh{1} = zeros(Ns,Ns,1);     % Human position transition(uncontrolable).
[n,m] = size(MAP);
for i = 1:n
    for j = 1:m
        
        % Allowable transitions from state s to state ss.
        %-----------------------------------------------------------------
        s     = sub2ind([n,m],i,j);
        for k = 1:nur
            try
                ss = sub2ind([n,m],i + ur(k,1),j + ur(k,2));
                Bh{1}(ss,s,1) = TPPr(k);     % Move at the action.
            catch
                
            end
        end
        ssum = sum(Bh{1},[1 3]);
        Bh{1}(s,s,1) = 1-ssum(s);            % Rest posibility.
    end
end


% Factor2: Human position
% ------------------------------------------------------------------------

Ps = 1;     % Precision of state transition.

Bh{2} = zeros(Ns,Ns,nuh);     % Robot position transition.
[n,m] = size(MAP);
for i = 1:n
    for j = 1:m
        
        % Allowable transitions from state s to state ss.
        %-----------------------------------------------------------------
        s     = sub2ind([n,m],i,j);
        for k = 1:nuh
            try
                ss = sub2ind([n,m],i + uh(k,1),j + uh(k,2));
                Bh{2}(ss,s,k) = Ps;         % Move at the policy.               
            catch
                Bh{2}(s ,s,k) = 1;
            end
        end
    end
end

bh{2} = Bh{2};


%% PREFERENCE %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
% Preferred outcomes: C and c
%=========================================================================
% C{outcome modality}(outcome,time point)
% The time point do not need to correspond with simulated time point T.

% No = zeros(1,numel(label.modality));    number of outcome at each outcome modality.

C{1}      = zeros(No(1),t);     % Robot location.
C{2}      = zeros(No(2),t);     % Human location.
C{3}      = zeros(No(3),t);     % Robot-Human relative location.
C{4}      = zeros(No(4),t);     % location(with the wall).

Cr = C;     % Initialize preference of both agents
Ch = C;


% ------------------------------------------------------------------------
% Preference in Robot generative model
% ------------------------------------------------------------------------


% o1: Preference regarding with goal direction
%-------------------------------------------------------------------------

PREFER_MAP_R = zeros(MAPw,MAPl);        % Mapping matrix indicating utility of prefered location.
PLr = 1/MAPl;                           % Most distant location indicate PLr*MAPl.

for i = 1:MAPl      % Prefered location about horizontal line.
    PREFER_MAP_R(:,i) = PLr*i;
end

for j = 1:No(1)     % Encoding preference at end time point.
    [n,m] = ind2sub(size(PREFER_MAP_R),j);
    Cr{1}(j,1) = PREFER_MAP_R(n,m);     % At this time, only encoded in at first time point.
end

PTr = 1;      % Robot wants to arrive at distant location in early time point(>1).

for k = 1:t     % Encoding preference at all time point.
    Cr{1}(:,k) = Cr{1}(:,1)/k;
end


% Thus, preference regarding with goal direction completely encoded(Cr{1}).


% o3: Preference regarding with collision avoidance
%-------------------------------------------------------------------------

PREFER_MAP_REL = zeros(MAPw*2-1,MAPl*2-1);      % Mapping matrix indicating risk of relative location.
mass = 30;   % Mass of the other agent.

for i = 1:No(3)     % Robot wants to avoid at close location with Human based on estimated risk.
    [n,m] = ind2sub(size(PREFER_MAP_REL),i);
    PREFER_MAP_REL(n,m) = -mass*W(n,m)/(R(n,m)+0.1);
end

PREFER_MAP_REL = fliplr(PREFER_MAP_REL);

for j = 1:No(3)     % Encoding risk(negative preference) at each relative location.
    [n,m] = ind2sub(size(PREFER_MAP_REL),j);
    Cr{3}(j,:) = PREFER_MAP_REL(n,m);
end


% Thus, preference regarding with collision avoidance completely encoded(Cr{3}).


% o4: Negative preference against wall
%-------------------------------------------------------------------------

for i = 1:No(4)
    [n,m] = ind2sub(size(PREFER_MAP_R),i);
    Cr{4}(i,:) = - MAP(n,m);     % Collision risk against wall
end


% Thus, preference regarding with wall position completely encoded(Cr{4}).



% ------------------------------------------------------------------------
% preference in Human generative model
% ------------------------------------------------------------------------


% o2: preference regarding with goal direction
%-------------------------------------------------------------------------

PREFER_MAP_H = zeros(MAPw,MAPl);        % Mapping matrix indicating utility of prefered location.
PLh = 1/MAPl;        % Most distant location indicate PLr*MAPl.

for i = 1:MAPl      % Prefered location about horizontal line.
    PREFER_MAP_H(:,i) = PLh*(MAPl-i+1);
end

for j = 1:No(2)     % Encoding preference at end time point.
    [n,m] = ind2sub(size(PREFER_MAP_H),j);
    Ch{2}(j,1) = PREFER_MAP_H(n,m);     % At this time, only encoded in at first time point.
end

PTh = 1;      % Human wants to arrive at distant location in early time point(>1).

for k = 1:t     % Encoding preference at all time point.
    Ch{2}(:,k) = Ch{2}(:,1)/k;
end


% Thus, preference regarding sith goal direction completely encoded(Ch{1})


% o3: Preference regarding with collision avoidance
%-------------------------------------------------------------------------

PREFER_MAP_REL = zeros(MAPw*2-1,MAPl*2-1);      % Mapping matrix indicating risk of relative location.
mass = 10;   % Mass of the other agent.

for i = 1:No(3)     % Human wants to avoid at close location with Robot based on estimated risk.
    [n,m] = ind2sub(size(PREFER_MAP_REL),i);
    PREFER_MAP_REL(n,m) = -mass*W(n,m)/(R(n,m)+0.1);
end

for j = 1:No(3)     % Encoding risk(negative preference) at each relative location.
    [n,m] = ind2sub(size(PREFER_MAP_REL),j);
    Ch{3}(j,:) = PREFER_MAP_REL(n,m);
end


% Thus, preference regarding with collision avoidance completely encoded(Ch{3})


% o4: Negative preference against wall
%-------------------------------------------------------------------------

for i = 1:No(4)
    [n,m] = ind2sub(size(PREFER_MAP_R),i);
    Ch{4}(i,:) = - MAP(n,m);     % collision risk against wall
end


% Thus, preference regarding with wall position completely encoded(Ch{4})


%% DEFINITION OF MDP STRUCTURE %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% ------------------------------------------------------------------------
% MDP structure corresponding to Human generative model
% ------------------------------------------------------------------------

mdpr.T = T;                    % Number of time steps(simulation until reflaction of generative model).
mdpr.U = Ur;                   % We could have instead used shallow policies (specifying U instead of V).

mdpr.A = Ar;                   % State-outcome mapping.
mdpr.B = Br;                   % Transition probabilities.
mdpr.C = Cr;                   % Preferred states.
mdpr.D = D;                    % Priors over initial states.

mdpr.b = br;                   % Lower case of the matrix distribution indicates generative model.


% ------------------------------------------------------------------------
% MDP structure corresponding to Robot generative model
% ------------------------------------------------------------------------

mdph.T = T;                    % Number of time steps(simulation until reflaction of generative model).
mdph.U = Uh;                   % We could have instead used shallow policies (specifying U instead of V).

mdph.A = Ah;                   % State-outcome mapping.
mdph.B = Bh;                   % Transition probabilities.
mdph.C = Ch;                   % Preferred states.
mdph.D = D;                    % Priors over initial states.

mdph.b = bh;                   % Lower case of the matrix distribution indicates generative model.


%% CONDUCTING SIMULATION %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% ------------------------------------------------------------------------
% Start time measurement.
% ------------------------------------------------------------------------

% tic


% ------------------------------------------------------------------------
% Initialize matrix for showing results.
% ------------------------------------------------------------------------

mdpr = spm_MDP_check(mdpr);     % Checking MDP structure
mdph = spm_MDP_check(mdph);


pathr = zeros(Tact+1,2,trialN);      % Path recorder
pathh = zeros(Tact+1,2,trialN);

optPathr = zeros(Tact+1,2,trialN);   % Optimal path recorder
optPathh = zeros(Tact+1,2,trialN);

px = 0:(MAPl-1);        % Horizontal axis for path.
py = (MAPw-1):-1:0;     % Vertical axis for path.

[yi,xi] = ind2sub(size(MAP),STARTr);     % Finding subscript of true state of Robot position at each time step.
pathr(1,1,:) = px(xi);
pathr(1,2,:) = py(yi);

[yi,xi] = ind2sub(size(MAP),STARTh);     % Finding subscript of true state of Human position at each time step.
pathh(1,1,:) = px(xi);
pathh(1,2,:) = py(yi);


% ------------------------------------------------------------------------
% Conducting the simulation: sequence of movements at each trial.
% ------------------------------------------------------------------------

Dtemp = D;

for k = 1:trialN    % Number of trials.

    rng('shuffle')

    mdprtemp(k) = mdpr;
    mdphtemp(k) = mdph;

    for i = 1:Tact  % Number of actual time steps.

        % ----------------------------------------------------------------
        % Proceed with subsequent moves.
        %-----------------------------------------------------------------

        MDPr(i,k)   = spm_MDP_VB_X(mdprtemp(k));        % Record model after 1 step simulation.
        MDPh(i,k)   = spm_MDP_VB_X(mdphtemp(k));        % Record model after 1 step simulation.


        % ----------------------------------------------------------------
        % Caluculating free energies and other index of internal state.
        %-----------------------------------------------------------------

        % Gh(:,i,k) = -MDPh(i,k).G(:,1);
        % Fh(:,i,k) = -MDPh(i,k).F(:,T);

        % Pprob(:,i,k) = MDPh(i,k).R(:,1);
        % vecprob(2:4) = Pprob(:,i,k);
       
        
        % ----------------------------------------------------------------
        % Reflecting outer world change after agents' actions
        % and recoding actual path of both agents.
        %-----------------------------------------------------------------

        mdprtemp(k).D{1}(:,1) = MDPr(i,k).X{1}(:,T);
        mdprtemp(k).D{2}(:,1) = MDPh(i,k).X{2}(:,T);
        mdphtemp(k).D{1}(:,1) = MDPr(i,k).X{1}(:,T);
        mdphtemp(k).D{2}(:,1) = MDPh(i,k).X{2}(:,T);

        [yi,xi] = ind2sub(size(MAP),MDPr(i,k).s(1,T));     % Finding subscript of true state of robot position at each time step.
        pathr(i+1,1,k) = px(xi);
        pathr(i+1,2,k) = py(yi);

        [yi,xi] = ind2sub(size(MAP),MDPh(i,k).s(2,T));     % Finding subscript of true state of human position at each time step.
        pathh(i+1,1,k) = px(xi);
        pathh(i+1,2,k) = py(yi);


        % ----------------------------------------------------------------
        % Display of steps and trials on command window.
        %-----------------------------------------------------------------

        Messtep = ['Agents arrive at ',num2str(i),' steps in ',num2str(k),' trials.'];
        steppat1 = sprintf('--------------- \n');
        disp(Messtep);
        disp(steppat1);

    end

    Mestrial = [num2str(k),' / ',num2str(trialN),' trials done.'];
    disp(Mestrial);
    trialpat1 = sprintf('=============== \n \n \n');
    disp(trialpat1);

end

% ------------------------------------------------------------------------
% Finish time measurement
% ------------------------------------------------------------------------

% toc
% realT = int16(toc);


%% SHOWING RESULTS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%


for i = 1:trialN
    optPathr(:,:,i)=PathSmoothing(pathr(:,:,i));
    optPathh(:,:,i)=PathSmoothing(pathh(:,:,i));
end

d_wall = 0.75;              % Tickness of the wall.
c_wall = [0.7 0.7 0.7];     % Color of the wall.


for k = 1:trialN

    Figure(k) = figure('Name','Result','Position',[500,100,1200,500]);

    % --------------------------------------------------------------------
    % Result: Figure of path.
    % --------------------------------------------------------------------

    plot(optPathr(:,1,k),optPathr(:,2,k),'-ob',LineWidth=1,MarkerSize=10);hold on;
    plot(optPathh(:,1,k),optPathh(:,2,k),'--sr',LineWidth=1,MarkerSize=10);hold on;
    plot(pathr(:,1,k),pathr(:,2,k),':c');hold on;
    plot(pathh(:,1,k),pathh(:,2,k),':m');hold on;

    axis([-0.5 MAPl-0.5 -0.5 MAPw-0.5])
    xticks(0:1:MAPl);
    yticks(0:1:MAPw);

    xlimit = get(gca, 'XLim');                                                  
    ylimit = get(gca, 'YLim');

    xlabel('moving direction',FontSize=14);
    ylabel('road width',FontSize=14);
    legend('ROBOT','HUMAN');
    title('Passing thorough each other',FontSize=16,FontWeight='bold');
    grid on;
    grid minor;

    Wall_coloring(xlimit,[ylimit(2)-d_wall,ylimit(2)-d_wall],ylimit(1)+d_wall);
    set(gca,'Color',c_wall);

    xtr = 0 - 0.4;
    ytr = MAPw - MAPwr - 0.2;
    strr = ['ROBOT',newline,'START'];
    text(xtr,ytr,strr,'Color','blue');

    xth = MAPl - 0.8;
    yth = MAPw - MAPwh - 0.2;
    strh = ['HUMAN',newline,'START'];
    text(xth,yth,strh,"Color",'red');

    % --------------------------------------------------------------------
    % It can be included other results of other variables in one figure
    % with using subplot command.
    % --------------------------------------------------------------------

end


% It can be saved as variables and figures with save command.


%% FUNCTIONS %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%

% ------------------------------------------------------------------------
% Making path smooth.
% It can be refered as "PathSmoothingSample.m"
% ------------------------------------------------------------------------

function optPath=PathSmoothing(path)
optPath=path;

alpha=0.5;
beta=0.2;

torelance=0.00001;
change=torelance;
while change>=torelance 
    change=0;
    for ip=2:(length(path(:,1))-1)
        prePath=optPath(ip,:);
        optPath(ip,:)=optPath(ip,:)-alpha*(optPath(ip,:)-path(ip,:));
        optPath(ip,:)=optPath(ip,:)-beta*(2*optPath(ip,:)-optPath(ip-1,:)-optPath(ip+1,:));
        change=change+norm(optPath(ip,:)-prePath);
    end
end

end


% ------------------------------------------------------------------------
% Making path result figure colored.
% It can be refered as "Square_coloring.m"
% ------------------------------------------------------------------------

function Wall_coloring(PX, PY, Bace, color)
hold on;

xlimit = get(gca, 'XLim');                                                  
ylimit = get(gca, 'YLim');                                                  
colordflt = [1.0 1.0 1.0];                                                  
if nargin<4, color = colordflt; end                                         
if nargin<3, Bace = ylimit(1); end                                          
if nargin<2, PY = [ylimit(2), ylimit(2)]; end                               
if nargin<1, PX = [xlimit(1), xlimit(2)]; end                               

Area_handle = area(PX , PY, Bace);                                          
hold off;                                                                   
set(Area_handle,'FaceColor', color);                                        
set(Area_handle,'LineStyle','none');                                        
set(Area_handle,'ShowBaseline','off');                                      
set(gca,'layer','top');                                                     
set(Area_handle.Annotation.LegendInformation, 'IconDisplayStyle','off');    
children_handle = get(gca, 'Children');                                     
set(gca, 'Children', circshift(children_handle,[-1 0]));                    

set(gca,'Xlim',xlimit);							    
set(gca,'Ylim',ylimit);							   
end

