# Selects one of the options, if wrong input Game Host keeps asking
def getChoice(i):
    greetingMessage = 'Game Host: Player {0} make your choice; rock, paper or scissors: '
    choice = raw_input(greetingMessage.format(i))
    while (choice.lower() != 'rock'
       and choice.lower() != 'paper'
       and choice.lower() != 'scissors'):
        choice = raw_input(greetingMessage.format(i))
    return choice

inputPlayer1 = getChoice(1);
inputPlayer2 = getChoice(2);

#Formula too see who wins
if inputPlayer1 == 'rock':
    if inputPlayer2 == 'paper':
        print 'Paper wraps rock \nPlayer 2 wins'
    elif inputPlayer2 == 'scissors':
        print 'Rock smashes scissors \nPlayer 1 wins'
    else:
        print "It's a draw"

if inputPlayer1 == 'scissors':
    if inputPlayer2 == 'paper':
        print 'Scissors cuts paper \nPlayer 1 wins'
    elif inputPlayer2 == 'rock':
        print 'Rock smashes scissors \nPlayer 2 wins'
    else:
        print "It's a draw"

if inputPlayer1 == 'paper':
    if inputPlayer2 == 'scissors':
        print 'Scissors cuts paper \nPlayer 2 wins'
    elif inputPlayer2 == 'rock':
        print 'Paper wraps rock \nPlayer 1 wins'
    else:
        print "It's a draw"