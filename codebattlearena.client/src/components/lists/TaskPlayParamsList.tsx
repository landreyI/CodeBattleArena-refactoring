import { TaskPlayParam } from "@/models/dbModels";
import { motion } from "framer-motion";
import { Check } from "lucide-react";
interface Props {
    params: TaskPlayParam[];
    cardWrapperClassName?: string;
    className?: string;
}

export function TaskPlayParamsList({ params, cardWrapperClassName, className }: Props) {

    return (
        <motion.div
            initial={{ opacity: 0, y: 10 }}
            animate={{ opacity: 1, y: 0 }}
            exit={{ opacity: 0, y: 10 }}
            transition={{ duration: 0.3 }}
            className={`flex flex-col gap-3 ${className}`}
        >
            {params.map((param) => (
                <div key={param.idParam} className={`flex flex-col md:flex-row justify-between gap-2 p-3 border rounded-xl bg-card ${cardWrapperClassName}`}>
                    <div className="flex flex-row gap-3">
                        <span className="text-sm font-medium text-muted-foreground">Key:</span>
                        <p className="text-base font-semibold">{param.paramKey}</p>
                    </div>
                    <div className="flex flex-row gap-3">
                        <span className="text-sm font-medium text-muted-foreground">Value:</span>
                        <p className="text-base font-semibold">{param.paramValue}</p>
                    </div>
                    <div className="flex items-center justify-start">
                        {param.isPrimary && (
                            <div className="flex items-center justify-center bg-green rounded-full">
                                <Check></Check>
                            </div>
                        )}
                    </div>
                </div>
            ))}
        </motion.div>
    );
}
export default TaskPlayParamsList;