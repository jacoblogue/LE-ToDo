"use client";

import { useEffect, useState } from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Card, CardContent } from "@/components/ui/card";
import { Checkbox } from "@/components/ui/checkbox";
import { Trash2, Plus, ListTodo } from "lucide-react";

const API_URL = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5203";

interface TodoItem {
  id: number;
  title: string;
  isCompleted: boolean;
  createdAt: string;
}

export default function Home() {
  const [todos, setTodos] = useState<TodoItem[]>([]);
  const [newTitle, setNewTitle] = useState("");
  const [loading, setLoading] = useState(true);
  const [adding, setAdding] = useState(false);

  const fetchTodos = async () => {
    try {
      const res = await fetch(`${API_URL}/api/todos`);
      if (res.ok) {
        const data = await res.json();
        setTodos(data);
      }
    } catch (error) {
      console.error("Failed to fetch todos:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchTodos();
  }, []);

  const addTodo = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!newTitle.trim()) return;

    setAdding(true);
    try {
      const res = await fetch(`${API_URL}/api/todos`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ title: newTitle }),
      });
      if (res.ok) {
        setNewTitle("");
        await fetchTodos();
      }
    } catch (error) {
      console.error("Failed to add todo:", error);
    } finally {
      setAdding(false);
    }
  };

  const deleteTodo = async (id: number) => {
    try {
      const res = await fetch(`${API_URL}/api/todos/${id}`, {
        method: "DELETE",
      });
      if (res.ok) {
        setTodos((prev) => prev.filter((t) => t.id !== id));
      }
    } catch (error) {
      console.error("Failed to delete todo:", error);
    }
  };

  return (
    <div className="min-h-screen bg-background">
      <div className="mx-auto max-w-2xl px-4 py-16">
        {/* Header */}
        <div className="mb-8 text-center">
          <div className="mb-2 flex items-center justify-center gap-3">
            <ListTodo className="h-8 w-8 text-primary" />
            <h1 className="text-4xl font-bold tracking-tight">LE-ToDo</h1>
          </div>
          <p className="text-muted-foreground">
            Keep track of your tasks, one at a time.
          </p>
        </div>

        {/* Add Task Form */}
        <form onSubmit={addTodo} className="mb-8 flex gap-2">
          <Input
            value={newTitle}
            onChange={(e) => setNewTitle(e.target.value)}
            placeholder="What needs to be done?"
            className="flex-1"
            disabled={adding}
          />
          <Button type="submit" disabled={adding || !newTitle.trim()}>
            <Plus className="mr-1 h-4 w-4" />
            Add
          </Button>
        </form>

        {/* Task List */}
        <div className="space-y-2">
          {loading ? (
            <p className="py-8 text-center text-muted-foreground">
              Loading tasks...
            </p>
          ) : todos.length === 0 ? (
            <Card>
              <CardContent className="py-12 text-center">
                <p className="text-muted-foreground">
                  No tasks yet. Add one above to get started!
                </p>
              </CardContent>
            </Card>
          ) : (
            todos.map((todo) => (
              <Card key={todo.id}>
                <CardContent className="flex items-center gap-3 py-3">
                  <Checkbox
                    checked={todo.isCompleted}
                    disabled
                    aria-label={`Task: ${todo.title}`}
                  />
                  <span
                    className={`flex-1 ${
                      todo.isCompleted
                        ? "text-muted-foreground line-through"
                        : ""
                    }`}
                  >
                    {todo.title}
                  </span>
                  <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => deleteTodo(todo.id)}
                    aria-label={`Delete ${todo.title}`}
                    className="text-muted-foreground hover:text-destructive"
                  >
                    <Trash2 className="h-4 w-4" />
                  </Button>
                </CardContent>
              </Card>
            ))
          )}
        </div>

        {/* Footer count */}
        {!loading && todos.length > 0 && (
          <p className="mt-4 text-center text-sm text-muted-foreground">
            {todos.length} task{todos.length !== 1 ? "s" : ""}
          </p>
        )}
      </div>
    </div>
  );
}
