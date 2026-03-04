import { TaskProgramming } from "@/models/dbModels";
import { TaskProgrammingMiniCard } from "../cards/TaskProgrammingMiniCard";
import { motion } from "framer-motion";
import IconButton from "../buttons/IconButton";
import { Trash2 } from "lucide-react";
interface Props {
    tasks: TaskProgramming[],
    cardWrapperClassName?: string;
    onDelete?: (taskId: number) => void;
}
export function TasksList({ tasks, cardWrapperClassName, onDelete }: Props) {

    return (
        <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 10 }}
            transition={{ duration: 0.3 }}
            className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-3">
            {tasks.map((task) => (
                <TaskProgrammingMiniCard
                    key={task.idTaskProgramming}
                    task={task}
                    className={cardWrapperClassName}
                >
                    {onDelete && (
                        <IconButton icon={<Trash2 className="w-4 h-4" />} onClick={() => onDelete(task.idTaskProgramming! ?? "")} />
                    )}
                </TaskProgrammingMiniCard>
            ))}
        </motion.div>
    );
}

export default TasksList;