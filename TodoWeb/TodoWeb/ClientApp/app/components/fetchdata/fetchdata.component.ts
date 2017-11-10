import { Component, Inject } from '@angular/core';
import { Http } from '@angular/http';

@Component({
    selector: 'fetchdata',
    templateUrl: './fetchdata.component.html'
})
export class FetchDataComponent {
    public todo: TodoItem[];
    public newTodo: string = '';

    constructor(public http: Http, @Inject('API_URL') public baseUrl: string) {
        this.fetch();
    }

    public fetch(): void {
        const uri = `${this.baseUrl}api/todo`;
        console.info(uri);
        this.http.get(uri).subscribe(result => {
            this.todo = result.json() as TodoItem[];
        }, error => console.error(error));
    }

    public completeClicked(todo: TodoItem): void {
        const uri = `${this.baseUrl}api/todo/${todo.id}`;
        console.info(uri);
        todo.isComplete = !todo.isComplete;
        this.http.put(uri, todo).subscribe(result => {
            this.fetch();
        }, error => console.error(error));
    }

    public newTodoItem(): void {
        if (this.newTodo && this.newTodo.length) {
            const uri = `${this.baseUrl}/api/todo`;
            this.http.post(uri, {
                name: this.newTodo,
                isComplete: false
            }).subscribe(result => {
                this.fetch();
                this.newTodo = '';
            }, error => console.error(error));
        }
    }

    public deleteItem(id: number): void {
        if (confirm("Are you sure you want to destroy this task forever?")) {
            const uri = `${this.baseUrl}/api/todo/${id}`;
            console.info(uri);
            this.http.delete(uri).subscribe(result => {
                this.fetch();
            }, error => console.error(error));
        }
    }
}

interface TodoItem {
    id: number;
    name: string;
    isComplete: boolean;
}
