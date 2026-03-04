import { TaskInputData } from "@/models/dbModels";
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs";
import { Card } from "@/components/ui/card";
import { useState } from "react";

interface Props {
    inputDatas: TaskInputData[];
    outDatas?: string[];
    cardWrapperClassName?: string;
}

export function InputDatasList({ inputDatas, outDatas, cardWrapperClassName = "" }: Props) {
    const displayed = inputDatas.slice(0, 5);
    const [activeTab, setActiveTab] = useState("0");

    return (
        <Card className="p-2 border-none h-full flex flex-col">
            <Tabs value={activeTab} onValueChange={setActiveTab} className="w-full h-full flex flex-col items-center">
                <TabsList className="flex flex-col md:flex-row mb-4 w-full h-auto px-1">
                    {displayed.map((_, index) => (
                        <TabsTrigger
                            key={index}
                            value={index.toString()}
                            className="w-full text-sm sm:text-base"
                        >
                            Case {index + 1}
                        </TabsTrigger>
                    ))}
                </TabsList>

                {displayed.map((item, index) => (
                    <TabsContent key={index} value={index.toString()} className="w-full h-full flex flex-col overflow-y-auto">
                        <div className={`space-y-4 ${cardWrapperClassName}`}>
                            <div>
                                <label className="text-muted-foreground text-sm sm:text-base">Input =</label>
                                <pre className="bg-muted rounded-md p-2 sm:p-3 mt-1 whitespace-pre-wrap break-words font-mono text-sm sm:text-base">
                                    {item.inputData?.data || "—"}
                                </pre>
                            </div>
                            <div>
                                <label className="text-muted-foreground text-sm sm:text-base">Expected Output =</label>
                                <pre className="bg-muted rounded-md p-2 sm:p-3 mt-1 whitespace-pre-wrap break-words font-mono text-sm sm:text-base text-primary">
                                    {item.answer || "—"}
                                </pre>
                            </div>
                            {outDatas && (
                                <div>
                                    <label className="text-muted-foreground text-sm sm:text-base">Received Output =</label>
                                    <pre className="bg-muted rounded-md p-2 sm:p-3 mt-1 whitespace-pre-wrap break-words font-mono text-sm sm:text-base">
                                        <span className={item.answer === outDatas[index] ? "text-green" : "text-red"}>
                                            {outDatas[index] || "—"}
                                        </span>
                                    </pre>
                                </div>
                            )}
                        </div>
                    </TabsContent>
                ))}
            </Tabs>
        </Card>

    );
}

export default InputDatasList;
