import React, { useEffect, useState } from "react";
import { TaskProgrammingFilters } from "@/models/filters";
import { Difficulty } from "@/models/dbModels";
import { useLangsProgramming } from "@/hooks/useLangsProgramming";
import EmptyState from "../common/EmptyState";
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from "@/components/ui/select";
import { Button } from "../ui/button";
import { Card } from "../ui/card";
import { Label } from "../ui/label";
import LoadingScreen from "../common/LoadingScreen";
import { getDifficultyColor } from "@/untils/helpers";

interface Props {
    filter: TaskProgrammingFilters;
    onChange: (filter: TaskProgrammingFilters) => void;
    handleSearch: () => void;
}

const getDifficulties = () => {
    return Object.keys(Difficulty) as Array<keyof typeof Difficulty>;
};

export function TaskFilter({ filter, onChange, handleSearch }: Props) {
    const difficulties = getDifficulties();
    const { langsProgramming, loading, error: langsError } = useLangsProgramming();

    // Локальное состояние для управления значениями <Select>
    const [selectedDifficulty, setSelectedDifficulty] = useState<string>(
        filter.difficulty || "all"
    );
    const [selectedLang, setSelectedLang] = useState<string>(filter.idLang?.toString() || "all");

    // Синхронизация локального состояния с пропсом filter
    useEffect(() => {
        setSelectedDifficulty(filter.difficulty || "all");
        setSelectedLang(filter.idLang?.toString() || "all");
    }, [filter]);

    const handleLangChange = (value: string) => {
        setSelectedLang(value);
    };

    const handleDifficultyChange = (value: string) => {
        setSelectedDifficulty(value);
    };

    const handleSearchClick = () => {
        const updatedFilter: TaskProgrammingFilters = {
            ...filter,
            idLang: selectedLang !== "all" ? Number(selectedLang) : undefined,
            difficulty: selectedDifficulty === "all" ? undefined : (selectedDifficulty as Difficulty),
        };
        onChange(updatedFilter);
        handleSearch();
    };

    if (loading) return <LoadingScreen />

    if (!langsProgramming || langsProgramming.length === 0) {
        return <EmptyState message="Langs not found" />;
    }

    return (
        <Card className="p-2 sm:p-4">
            <div className="flex flex-col gap-4 md:flex-row md:items-center md:gap-6">
                <div className="flex flex-col gap-4 md:flex-row md:gap-6">
                    <div className="flex flex-col gap-1">
                        <Label className="text-sm sm:text-base font-medium">
                            Language
                        </Label>
                        <Select
                            value={selectedLang}
                            onValueChange={handleLangChange}
                            disabled={loading}
                        >
                            <SelectTrigger className="w-full md:w-[180px] border-gray-300 focus:ring-2 focus:ring-blue-500">
                                <SelectValue placeholder="All languages" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="all">All languages</SelectItem>
                                {langsProgramming.map((lang) => (
                                    <SelectItem key={lang.idLang} value={lang.idLang.toString()}>
                                        {lang.nameLang}
                                    </SelectItem>
                                ))}
                            </SelectContent>
                        </Select>
                        {langsError && (
                            <span className="text-sm text-red-600">Error loading languages</span>
                        )}
                    </div>

                    <div className="flex flex-col gap-1">
                        <Label className="text-sm sm:text-base font-medium">
                            Difficulty
                        </Label>
                        <Select
                            value={selectedDifficulty}
                            onValueChange={handleDifficultyChange}
                        >
                            <SelectTrigger className="w-full md:w-[180px] border-gray-300 focus:ring-2 focus:ring-blue-500">
                                <SelectValue placeholder="All difficulties" />
                            </SelectTrigger>
                            <SelectContent>
                                <SelectItem value="all">All difficulties</SelectItem>
                                {difficulties.map((diff) => (
                                    <SelectItem key={diff} value={diff} className={`${getDifficultyColor(diff)} mt-1`}>
                                        {diff}
                                    </SelectItem>
                                ))}
                            </SelectContent>
                        </Select>
                    </div>
                </div>
                <Button
                    className="w-full md:w-auto btn-animation px-4 py-2"
                    onClick={handleSearchClick}
                >
                    Search
                </Button>
            </div>
        </Card>
    );
}

export default TaskFilter;