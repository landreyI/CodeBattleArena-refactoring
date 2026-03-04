import Editor from "@monaco-editor/react";
import { useTheme } from "@/contexts/ThemeContext";
import { useEffect, useRef, useState } from "react";

interface Props {
    code: string;
    onChange?: (value: string) => void;
    language: string;
    maxHeight?: number;
    readonly?: boolean;
    autoResize?: boolean;
    className?: string;
}

export function CodeViewer({
    code,
    onChange,
    language,
    maxHeight = 400,
    readonly = true,
    autoResize,
    className,
}: Props) {
    const { isDarkMode } = useTheme();
    const editorRef = useRef<any>(null);
    const monacoRef = useRef<any>(null);
    const [height, setHeight] = useState<number | string>(autoResize ? 100 : "100%");

    const handleEditorDidMount = (editor: any, monaco: any) => {
        editorRef.current = editor;
        monacoRef.current = monaco;

        monaco.editor.setTheme(isDarkMode ? "vs-dark" : "vs");

        if (autoResize) {
            const lineHeight = editor.getOption(monaco.editor.EditorOption.lineHeight);
            const lineCount = editor.getModel()?.getLineCount() ?? 1;
            const padding = 20;
            const newHeight = Math.min(lineCount * lineHeight + padding, maxHeight);
            setHeight(newHeight);
            editor.layout();
        }
    };

    useEffect(() => {
        if (monacoRef.current) {
            monacoRef.current.editor.setTheme(isDarkMode ? "vs-dark" : "vs");
        }
    }, [isDarkMode]);

    return (
        <div
            className={`overflow-hidden shadow-sm ${className}`}
            style={{
                maxHeight: autoResize ? maxHeight : undefined,
                height: autoResize ? undefined : "100%",
                overflowY: "auto",
            }}
        >
            <Editor
                height={height}
                language={language}
                value={code}
                onChange={(val) => {
                    if (!readonly && onChange) {
                        onChange(val ?? "");
                    }
                }}
                theme={isDarkMode ? "vs-dark" : "vs"}
                onMount={handleEditorDidMount}
                options={{
                    readOnly: readonly,
                    minimap: { enabled: false },
                    scrollBeyondLastLine: false,
                    lineNumbers: "off",
                    fontSize: 14,
                    wordWrap: "on",
                    padding: { top: 10, bottom: 10 },
                    overviewRulerLanes: 0,
                }}
            />
        </div>
    );
}

export default CodeViewer;
