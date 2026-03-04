import { useEffect, useState } from "react";
import { PlayerFilters } from "@/models/filters";
import { Button } from "../ui/button";
import { Card } from "../ui/card";
import { Input } from "../ui/input";
import { Label } from "../ui/label";
import { Role } from "@/models/dbModels";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "../ui/select";
import { useAuth } from "@/contexts/AuthContext";
import { isEditRole } from "../../untils/businessRules";

interface Props {
    filter: PlayerFilters;
    onChange: (filter: PlayerFilters) => void;
    handleSearch: () => void;
}

export function ItemFilter({ filter, onChange, handleSearch }: Props) {
    const [selectedRole, setSelectedRole] = useState<string>(filter.role || "all");
    const [userName, setUserName] = useState<string>(filter.userName || "");
    const { user } = useAuth();

    useEffect(() => {
        setSelectedRole(filter.role || "all");
        setUserName(filter.userName || "");
    }, [filter]);

    const handleRoleChange = (value: string) => {
        setSelectedRole(value);
    };

    const handleUserNameChange = (value: string) => {
        setUserName(value);
    };

    const handleSearchClick = () => {
        const updatedFilter: PlayerFilters = {
            ...filter,
            role: selectedRole === "all" ? undefined : (selectedRole as Role),
            userName: userName.trim() !== "" ? userName.trim() : undefined,
        };
        onChange(updatedFilter);
        handleSearch();
    };

    return (
        <Card className="p-2 sm:p-4">
            <div className="flex flex-col gap-4 md:flex-row md:items-center md:gap-6 flex-wrap">
                <div className="flex flex-col gap-4 md:flex-row md:gap-6">
                    {isEditRole(user?.roles ?? []) && (
                        <div className="flex flex-col gap-1">
                            <Label className="text-sm font-medium">Roles</Label>
                            <div className="flex flex-row gap-1">
                                <Select
                                    value={selectedRole}
                                    onValueChange={handleRoleChange}
                                >
                                    <SelectTrigger className="w-full md:w-[180px] border-gray-300 focus:ring-2 focus:ring-blue-500">
                                        <SelectValue placeholder="All types" />
                                    </SelectTrigger>
                                    <SelectContent>
                                        <SelectItem value="all">All types</SelectItem>
                                        {Object.values(Role).map((role) => (
                                            <SelectItem key={role} value={role}>
                                                {role}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            </div>
                        </div>
                    )}
                    <div className="flex flex-col gap-1">
                        <Label className="text-sm font-medium">Username</Label>
                        <Input
                            placeholder="Enter username"
                            value={userName}
                            onChange={(e) => handleUserNameChange(e.target.value)}
                            className="w-full"
                        />
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


export default ItemFilter;