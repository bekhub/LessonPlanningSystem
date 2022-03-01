
Action actions = () => Console.WriteLine("Hello world");
actions += () => Console.WriteLine("bye bye");

DoSmth(actions);

void DoSmth(Action action) => action?.Invoke();
