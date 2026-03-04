import { GenericDropdownMenu } from "./GenericDropdownMenu";

export function TaskMenu() {
    return (
        <GenericDropdownMenu
            triggerContent={<button className="nav-link">Task</button>}
            menuLabel="Actions with tasks"
            actions={[
                { label: "Tasks List", href: "/task/list-task", shortcut: "⌘T" },
            ]}
        />
    );
}

export default TaskMenu;