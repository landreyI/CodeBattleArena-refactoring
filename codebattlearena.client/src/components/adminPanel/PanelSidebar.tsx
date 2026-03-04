
import { ChevronRight, ChevronsUpDown, Edit, LucideIcon, Search, ShieldCheck, Users } from "lucide-react"
import { SidebarGroupLabel, SidebarMenu, SidebarMenuButton, SidebarMenuItem, SidebarMenuSub, SidebarMenuSubItem, SidebarContent, SidebarHeader, Sidebar, SidebarFooter, SidebarTrigger } from "../ui/sidebar"
import { Collapsible, CollapsibleContent, CollapsibleTrigger } from "../ui/collapsible"
import { MenuAction } from "../menu/GenericDropdownMenu"
import { useAuth } from "@/contexts/AuthContext"
import { isEditRole, isModerationRole } from "@/untils/businessRules"
import UserMenu from "../menu/UserMenu"
import { useNavigate } from "react-router-dom"
import { getRoleColor } from "@/untils/helpers"
import { Badge } from "../ui/badge"
import { Role } from "@/models/dbModels"

export const itemsStaff: MenuAction[] = [
    { label: "Admins", href: `/player/list-players?role=${Role.Admin}`, shortcut: "" },
    { label: "Managers", href: `/player/list-players?role=${Role.Manager}`, shortcut: "" },
    { label: "Moderators", href: `/player/list-players?role=${Role.Moderator}`, shortcut: "" },
];

export const itemsEdit: MenuAction[] = [
    { label: "Create Task", href: "/task/create-task", shortcut: "+⌘T" },
    { label: "Generate Task", href: "/task/ai-generate-task/null", shortcut: "+⌘AI" },
    { label: "Create League", href: "/league/create-league", shortcut: "+⌘L" },
    { label: "Create Item", href: "/item/create-item", shortcut: "+⌘I" },
    { label: "Create Reward", href: "/quest/create-reward", shortcut: "+⌘R" },
    { label: "Create Quest", href: "/quest/create-quest", shortcut: "+⌘Q" },
]

export const itemsModeration: MenuAction[] = [
    { label: "Player Reports", href: "/moderation/player-reports", shortcut: "" },
    { label: "Session Logs", href: "/moderation/session-logs", shortcut: "" },
    { label: "Moderation History", href: "/moderation/history", shortcut: "" },
];

export const itemsSearch: MenuAction[] = [
    { label: "List Rewards", href: "/quest/list-rewards", shortcut: "" },
]


export function PanelSidebar() {
    const { user, logout } = useAuth();
    const isEdit = isEditRole(user?.roles ?? []);
    const isModerator = isModerationRole(user?.roles ?? []);

    const navigate = useNavigate();
    const handleLogout = (e: React.MouseEvent) => {
        e.preventDefault();
        logout();
        navigate("/home");
    };

    if (!user || (!isEdit && !isModerator))
        return null;

    const mainItemsCollapsible: {
        title: string;
        isActive?: boolean;
        items?: MenuAction[];
        icon?: LucideIcon;
    }[] = [
            { title: "Staff", items: itemsStaff, icon: Users },
            ...(isEdit ? [{ title: "Edit", isActive: true, items: itemsEdit, icon: Edit }] : []),
            ...(isModerator ? [{ title: "Moderation", isActive: true, items: itemsModeration, icon: ShieldCheck }] : []),
            { title: "Search", isActive: true, items: itemsSearch, icon: Search },
        ];

    return (
        <Sidebar collapsible="icon" className="h-full">
            <SidebarHeader className="flex flex-row">
                <SidebarTrigger className="bg-blue p-1 rounded-lg" iconSize={25} />
                <SidebarGroupLabel className="text-sm group-data-[collapsible=icon]:hidden">Admin Panel</SidebarGroupLabel>
            </SidebarHeader>
            <SidebarContent>
                <div className="overflow-auto h-full min-h-0 p-2">
                    <SidebarMenu>
                        {mainItemsCollapsible.map((itemCollapsible) => (
                            <Collapsible className="group/collapsible" defaultOpen={itemCollapsible.isActive} key={itemCollapsible.title}>
                                <SidebarMenuItem>
                                    <CollapsibleTrigger asChild>
                                        <SidebarMenuButton tooltip={itemCollapsible.title}>
                                            {itemCollapsible.icon && <itemCollapsible.icon size={20} />}
                                            <span>{itemCollapsible.title}</span>
                                            <ChevronRight className="ml-auto transition-transform duration-200 group-data-[state=open]/collapsible:rotate-90" />
                                        </SidebarMenuButton>
                                    </CollapsibleTrigger>
                                    <CollapsibleContent>
                                        <SidebarMenuSub>
                                            {itemCollapsible?.items?.map((item) => (
                                                <SidebarMenuItem key={item.label}>
                                                    <SidebarMenuButton asChild>
                                                        <a href={item.href} className="flex justify-between">
                                                            <span>{item.label}</span>
                                                            {item.shortcut}
                                                        </a>
                                                    </SidebarMenuButton>
                                                </SidebarMenuItem>
                                            ))}
                                        </SidebarMenuSub>
                                    </CollapsibleContent>
                                </SidebarMenuItem>
                            </Collapsible>
                        ))}
                        <SidebarMenuButton asChild className="group-data-[collapsible=icon]:hidden">
                            <SidebarMenuSubItem>Chart</SidebarMenuSubItem>
                        </SidebarMenuButton>
                    </SidebarMenu>
                </div>

                <SidebarFooter className="p-2">
                    <div className="flex flex-col justify-between item-center hover:bg-muted rounded-xl p-2 group-data-[collapsible=icon]:p-0">
                        <div className="flex flex-row justify-between">
                            <UserMenu user={user} handleLogout={handleLogout} className="w-full" />
                            <ChevronsUpDown size="18" className="mt-1 group-data-[collapsible=icon]:hidden" />
                        </div>
                        <div className="flex flex-wrap gap-2 mt-2 group-data-[collapsible=icon]:hidden">
                            {user.roles?.map((role, index) => (
                                <Badge key={index} className={getRoleColor(role)}>
                                    {role}
                                </Badge>
                            ))}
                        </div>
                    </div>
                </SidebarFooter>
            </SidebarContent>
        </Sidebar>
    )
}

export default PanelSidebar;