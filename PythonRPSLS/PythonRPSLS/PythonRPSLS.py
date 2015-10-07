# Selects one of the options, if wrong input Game Host keeps asking
def getChoice(i):
    greetingMessage = 'Game Host: Player {0} make your choice; rock, paper, scissors, lizard or spock: '
    choice = raw_input(greetingMessage.format(i))
    while (choice.lower() != 'rock'
       and choice.lower() != 'paper'
       and choice.lower() != 'scissors'
       and choice.lower() != 'lizard'
       and choice.lower() != 'spock'):
        choice = raw_input(greetingMessage.format(i))
    return choice

inputPlayer1 = getChoice(1);
inputPlayer2 = getChoice(2);

#Formula too see who wins
if inputPlayer1 == 'rock':
    if inputPlayer2 == 'paper':
        print 'Paper wraps rock \nPlayer 2 wins'
    elif inputPlayer2 == 'spock':
        print 'Spock vaporizes rock \nPlayer 2 wins'
    elif inputPlayer2 == 'scissors':
        print 'Rock smashes scissors \nPlayer 1 wins'
    elif inputPlayer2 == 'lizard':
        print 'Rock crushes lizard \nPlayer 1 wins'
    else:
        print "It's a draw"

if inputPlayer1 == 'scissors':
    if inputPlayer2 == 'spock':
        print 'Spock smashes scissors \nPlayer 2 winss'
    elif inputPlayer2 == 'rock':
        print 'Rock crushes scissors \nPlayer 2 wins'
    elif inputPlayer2 == 'lizard':
        print 'Scissors cut lizard \nPlayer 1 wins'
    elif inputPlayer2 == 'paper':
        print 'Scissors cuts paper \nPlayer 1 wins'
    else:
        print "It's a draw"

if inputPlayer1 == 'paper':
    if inputPlayer2 == 'scissors':
        print 'Scissors cuts paper \nPlayer 2 wins'
    elif inputPlayer2 == 'lizard':
        print 'Lizard eats paper \nPlayer 2 wins'
    elif inputPlayer2 == 'rock':
        print 'Paper wraps rock \nPlayer 1 wins'
    elif inputPlayer2 == 'spock':
        print 'Paper disaproves spock \nPlayer 1 wins'
    else:
        print "It's a draw"

if inputPlayer1 == 'spock':
    if inputPlayer2 == 'paper':
        print 'Paper disaproves spock \nPlayer 2 wins'
    elif inputPlayer2 == 'lizard':
        print 'Lizard poisons spock \nPlayer 2 wins'
    elif inputPlayer2 == 'scissors':
        print 'Spock smashes scissors \nPlayer 1 wins'
    elif inputPlayer2 == 'rock':
        print 'Spock vaporizes rock \nPlayer 1 wins'
    else:
        print "It's a draw"

if inputPlayer1 == 'lizard':
    if inputPlayer2 == 'rock':
        print 'Rock crushes lizard \nPlayer 2 wins'
    elif inputPlayer2 == 'scissors':
        print 'Scissors cut lizard \nPlayer 2 wins'
    elif inputPlayer2 == 'paper':
        print 'Lizard eats paper \nPlayer 1 wins'
    elif inputPlayer2 == 'spock':
        print 'Lizard poisons spock \nPlayer 1 wins'
    else:
        print "It's a draw"