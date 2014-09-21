// to simplify, yet another level of complexity, make interfaces to expose 
// certain elements of action.  Ex: currently lists are used to identify
// what things can be attacked, or have cast fear into.
// If we add an Interface, such as to ANY commonAI (human, guard, zombie)
// or even non commonAI (safe zone), that can be attacked, all we need to 
// know is they can all "takeDamage"

public interface ICanBeAttacked
{
	int TakeDamage( float attackDamage );
}

// similarly for being scared.  At present, only humans can be "scared"
public interface ICanBeScared
{
	void Afraid();
}

